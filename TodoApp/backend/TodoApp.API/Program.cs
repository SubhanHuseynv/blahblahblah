using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using TodoApp.API.Data;
using TodoApp.API.Middlewares;
using TodoApp.API.Repositories;
using TodoApp.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Render (və oxşar platformalar) portu PORT env dəyişəni ilə verir
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("ConnectionStrings:Default is not configured.");

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(ConnectionStringHelper.Normalize(connectionString)));

builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddScoped<ITodoService, TodoService>();

// İcazəli origin-lər config-dən gəlir (env: Cors__AllowedOrigins__0, Cors__AllowedOrigins__1 ...)
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
const string CorsPolicy = "ReactClient";
builder.Services.AddCors(opt => opt.AddPolicy(CorsPolicy, policy =>
    policy.WithOrigins(allowedOrigins)
          .AllowAnyHeader()
          .AllowAnyMethod()));

builder.Services.AddHealthChecks();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Render HTTPS-i öz proxy-sində bitirir, real scheme/IP header-lərdən oxunur
builder.Services.Configure<ForwardedHeadersOptions>(opt =>
{
    opt.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    opt.KnownNetworks.Clear();
    opt.KnownProxies.Clear();
});

var app = builder.Build();

// Sadəlik üçün: migration əvəzinə cədvəllər yoxdursa avtomatik yaradılır.
// DB container-i hələ hazır olmaya bilər, ona görə bir neçə dəfə yenidən cəhd edilir.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    for (var attempt = 1; ; attempt++)
    {
        try
        {
            db.Database.EnsureCreated();
            break;
        }
        catch (Exception ex) when (attempt < 10)
        {
            logger.LogWarning("Database not ready (attempt {Attempt}/10): {Message}", attempt, ex.Message);
            await Task.Delay(TimeSpan.FromSeconds(3));
        }
    }
}

app.UseForwardedHeaders();

if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("Swagger:Enabled"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseCors(CorsPolicy);
app.MapHealthChecks("/health");
app.MapControllers();

app.Run();

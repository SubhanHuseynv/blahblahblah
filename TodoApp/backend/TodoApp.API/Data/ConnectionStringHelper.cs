using Npgsql;

namespace TodoApp.API.Data;

public static class ConnectionStringHelper
{
    /// <summary>
    /// Render (və Heroku, Neon və s.) bağlantını "postgres://user:pass@host:port/db" URL formatında verir.
    /// Npgsql isə "Host=...;Username=..." formatını gözləyir — burada çevrilir.
    /// </summary>
    public static string Normalize(string connectionString)
    {
        if (!connectionString.StartsWith("postgres://") && !connectionString.StartsWith("postgresql://"))
            return connectionString;

        var uri = new Uri(connectionString);
        var userInfo = uri.UserInfo.Split(':', 2);

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.IsDefaultPort || uri.Port <= 0 ? 5432 : uri.Port,
            Database = uri.AbsolutePath.TrimStart('/'),
            Username = Uri.UnescapeDataString(userInfo[0]),
            Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : null,
            SslMode = SslMode.Prefer
        };
        return builder.ConnectionString;
    }
}

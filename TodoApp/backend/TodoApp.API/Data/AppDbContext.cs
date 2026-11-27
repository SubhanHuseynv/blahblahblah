using Microsoft.EntityFrameworkCore;
using TodoApp.API.Entities;

namespace TodoApp.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<TodoItem> Todos => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoItem>(b =>
        {
            b.Property(t => t.Title).IsRequired().HasMaxLength(200);
        });
    }
}

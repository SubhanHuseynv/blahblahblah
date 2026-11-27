using Microsoft.EntityFrameworkCore;
using TodoApp.API.Data;
using TodoApp.API.Entities;

namespace TodoApp.API.Repositories;

public class TodoRepository : ITodoRepository
{
    private readonly AppDbContext _context;

    public TodoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<TodoItem>> GetAllAsync() =>
        await _context.Todos.AsNoTracking().OrderByDescending(t => t.Id).ToListAsync();

    public async Task<TodoItem?> GetByIdAsync(long id) =>
        await _context.Todos.FirstOrDefaultAsync(t => t.Id == id);

    public void Add(TodoItem entity) => _context.Todos.Add(entity);

    public void Update(TodoItem entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Todos.Update(entity);
    }

    public void Delete(TodoItem entity) => _context.Todos.Remove(entity);

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}

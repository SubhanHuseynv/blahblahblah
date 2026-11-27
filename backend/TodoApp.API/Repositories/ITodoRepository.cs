using TodoApp.API.Entities;

namespace TodoApp.API.Repositories;

public interface ITodoRepository
{
    Task<IReadOnlyList<TodoItem>> GetAllAsync();
    Task<TodoItem?> GetByIdAsync(long id);
    void Add(TodoItem entity);
    void Update(TodoItem entity);
    void Delete(TodoItem entity);
    Task SaveChangesAsync();
}

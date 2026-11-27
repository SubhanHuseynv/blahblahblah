using TodoApp.API.Dtos;
using TodoApp.API.Entities;
using TodoApp.API.Exceptions;
using TodoApp.API.Repositories;

namespace TodoApp.API.Services;

public class TodoService : ITodoService
{
    private readonly ITodoRepository _repository;

    public TodoService(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<GetTodoDto>> GetAllAsync()
    {
        var todos = await _repository.GetAllAsync();
        return todos.Select(ToDto).ToList();
    }

    public async Task<GetTodoDto> GetByIdAsync(long id) => ToDto(await FindAsync(id));

    public async Task<GetTodoDto> PostAsync(PostTodoDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new BadRequestException("Title cannot be empty.");

        var todo = new TodoItem { Title = dto.Title.Trim() };
        _repository.Add(todo);
        await _repository.SaveChangesAsync();
        return ToDto(todo);
    }

    public async Task PutAsync(long id, PutTodoDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new BadRequestException("Title cannot be empty.");

        var todo = await FindAsync(id);
        todo.Title = dto.Title.Trim();
        todo.IsCompleted = dto.IsCompleted;
        _repository.Update(todo);
        await _repository.SaveChangesAsync();
    }

    public async Task ToggleAsync(long id)
    {
        var todo = await FindAsync(id);
        todo.IsCompleted = !todo.IsCompleted;
        _repository.Update(todo);
        await _repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var todo = await FindAsync(id);
        _repository.Delete(todo);
        await _repository.SaveChangesAsync();
    }

    private async Task<TodoItem> FindAsync(long id) =>
        await _repository.GetByIdAsync(id)
        ?? throw new NotFoundException($"Todo with ID {id} not found.");

    private static GetTodoDto ToDto(TodoItem t) => new(t.Id, t.Title, t.IsCompleted, t.CreatedAt);
}

using TodoApp.API.Dtos;

namespace TodoApp.API.Services;

public interface ITodoService
{
    Task<IReadOnlyList<GetTodoDto>> GetAllAsync();
    Task<GetTodoDto> GetByIdAsync(long id);
    Task<GetTodoDto> PostAsync(PostTodoDto dto);
    Task PutAsync(long id, PutTodoDto dto);
    Task ToggleAsync(long id);
    Task DeleteAsync(long id);
}

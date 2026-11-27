using System.ComponentModel.DataAnnotations;

namespace TodoApp.API.Dtos;

public record GetTodoDto(long Id, string Title, bool IsCompleted, DateTime CreatedAt);

public record PostTodoDto(
    [Required, MaxLength(200)] string Title);

public record PutTodoDto(
    [Required, MaxLength(200)] string Title,
    bool IsCompleted);

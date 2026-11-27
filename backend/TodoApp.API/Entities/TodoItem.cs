namespace TodoApp.API.Entities;

public class TodoItem : BaseEntity
{
    public string Title { get; set; } = null!;
    public bool IsCompleted { get; set; }
}

namespace ToDo.Configuration;

public record ToDoApp
{
	public bool? IsDark { get; init; }
	public string? LastTaskList { get; init; }
}

namespace ToDo;

public class ToDoTaskListRequestData
{
	[JsonPropertyName("displayName")]
	public string? DisplayName { get; set; }
}

namespace ToDo;

public class TaskListRequestData
{
	[JsonPropertyName("displayName")]
	public string? DisplayName { get; set; }
}

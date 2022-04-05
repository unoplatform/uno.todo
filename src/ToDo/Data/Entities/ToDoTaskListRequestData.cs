
using System.Text.Json.Serialization;

namespace ToDo;

public class ToDoTaskListRequestData
{
	[JsonPropertyName("displayName")]
	public string? DisplayName { get; set; }
}

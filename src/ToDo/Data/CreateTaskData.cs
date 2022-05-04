namespace ToDo;

public class CreateTaskData
{
	[JsonPropertyName("importance")]
	public string? Importance { get; set; }

	[JsonPropertyName("isReminderOn")]
	public bool IsReminderOn { get; set; }

	[JsonPropertyName("status")]
	public string? Status { get; set; }

	[JsonPropertyName("title")]
	public string? Title { get; set; }

	[JsonPropertyName("body")]
	public TaskBodyData? Body { get; set; }

	[JsonPropertyName("dueDateTime")]
	public DateTimeData? DueDateTime { get; set; }
}

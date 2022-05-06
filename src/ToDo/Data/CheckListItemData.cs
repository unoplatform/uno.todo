namespace ToDo;

public record CheckListItemData
{
	[JsonPropertyName("id")]
	public string? Id { get; set; }

	[JsonPropertyName("isChecked")]
	public bool IsChecked { get; set; }

	[JsonPropertyName("displayName")]
	public string? DisplayName { get; set; }

	[JsonPropertyName("createdDateTime")]
	public DateTime CreatedDateTime { get; set; }

	[JsonPropertyName("checkedDateTime")]
	public DateTime? CheckedDateTime { get; set; }

	[JsonPropertyName("taskId")]
	public string? TaskId { get; set; }
}

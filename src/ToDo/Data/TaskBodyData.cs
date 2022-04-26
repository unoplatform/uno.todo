namespace ToDo;

public record TaskBodyData
{
	[JsonPropertyName("content")]
	public string? Content { get; set; }

	[JsonPropertyName("contentType")]
	public string? ContentType { get; set; }
}

﻿namespace ToDo;

public class ToDoTaskBodyData
{
	[JsonPropertyName("content")]
	public string? Content { get; set; }

	[JsonPropertyName("contentType")]
	public string? ContentType { get; set; }
}

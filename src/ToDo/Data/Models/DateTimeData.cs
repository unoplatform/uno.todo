﻿namespace ToDo.Data.Models;

public record DateTimeData
{
	[JsonPropertyName("dateTime")]
	public DateTime DateTime { get; set; }

	[JsonPropertyName("timeZone")]
	public string? TimeZone { get; set; }
}

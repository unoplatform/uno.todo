namespace ToDo.Data.Models;

public class DateTimeData
{
	[JsonPropertyName("dateTime")]
	public DateTime DateTime { get; set; }

	[JsonPropertyName("timeZone")]
	public string? TimeZone { get; set; }
}

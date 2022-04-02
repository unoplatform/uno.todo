using Newtonsoft.Json;

namespace ToDo;
public class DateTimeData
{
    [JsonProperty("dateTime")]
    public DateTime DateTime { get; set; }

    [JsonProperty("timeZone")]
    public string? TimeZone { get; set; }
}

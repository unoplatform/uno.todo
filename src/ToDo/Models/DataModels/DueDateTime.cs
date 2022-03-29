using Newtonsoft.Json;

namespace ToDo.Models.DataModels
{
    public class DueDateTime
    {
        [JsonProperty("dateTime")]
        public DateTime DateTime { get; set; }

        [JsonProperty("timeZone")]
        public string? TimeZone { get; set; }
    }
}

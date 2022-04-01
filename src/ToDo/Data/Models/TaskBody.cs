using Newtonsoft.Json;

namespace ToDo.Data.Models
{
    public class TaskBody
    {
        [JsonProperty("content")]
        public string? Content { get; set; }

        [JsonProperty("contentType")]
        public string? ContentType { get; set; }
    }
}

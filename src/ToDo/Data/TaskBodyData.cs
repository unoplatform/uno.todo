using Newtonsoft.Json;

namespace ToDo;
public class TaskBodyData
{
    [JsonProperty("content")]
    public string? Content { get; set; }

    [JsonProperty("contentType")]
    public string? ContentType { get; set; }
}

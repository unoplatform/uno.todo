
using System.Text.Json.Serialization;

namespace ToDo;
public class TaskBodyData
{
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("contentType")]
    public string? ContentType { get; set; }
}

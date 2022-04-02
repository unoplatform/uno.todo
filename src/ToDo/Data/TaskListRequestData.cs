using Newtonsoft.Json;

namespace ToDo;

public class TaskListRequestData
{
    [JsonProperty(PropertyName = "displayName")]
    public string? DisplayName { get; set; }
}

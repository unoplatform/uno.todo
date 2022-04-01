using Newtonsoft.Json;

namespace ToDo.Data.Models;

public class TaskListRequestData
{
    [JsonProperty(PropertyName = "displayName")]
    public string? DisplayName { get; set; }
}

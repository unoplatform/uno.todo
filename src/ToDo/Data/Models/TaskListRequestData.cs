using Newtonsoft.Json;

namespace ToDo.Data.Models;

public record TaskListRequestData
{
    [JsonProperty(PropertyName = "displayName")]
    public string? DisplayName { get; set; }
}

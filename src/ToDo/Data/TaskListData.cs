using Newtonsoft.Json;

namespace ToDo;

public class TaskListData
{
    [JsonProperty(PropertyName = "@odata.etag")]
    public string? Odata { get; set; }

    [JsonProperty(PropertyName = "id")]
    public string? Id { get; set; }

    [JsonProperty(PropertyName = "displayName")]
    public string? DisplayName { get; set; }

    [JsonProperty(PropertyName = "isOwner")]
    public bool IsOwner { get; set; }

    [JsonProperty(PropertyName = "isShared")]
    public bool IsShared {get; set; }

    [JsonProperty(PropertyName = "wellknownListName")]
    public string? WellknownListName { get; set; }
}

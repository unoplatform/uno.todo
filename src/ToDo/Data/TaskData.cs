using Newtonsoft.Json;

namespace ToDo;
public class TaskData
{
    [JsonProperty("id")]
    public string? Id { get; set; }

    [JsonProperty("importance")]
    public string? Importance { get; set; }

    [JsonProperty("isReminderOn")]
    public bool IsReminderOn { get; set; }

    [JsonProperty("status")]
    public string? Status { get; set; }

    [JsonProperty("title")]
    public string? Title { get; set; }

    [JsonProperty("createdDateTime")]
    public DateTime CreatedDateTime { get; set; }

    [JsonProperty("lastModifiedDateTime")]
    public DateTime LastModifiedDateTime { get; set; }

    [JsonProperty("body")]
    public TaskBodyData? Body { get; set; }

    [JsonProperty("dueDateTime")]
    public DateTimeData? DueDateTime { get; set; }

    [JsonProperty("linkedResources@odata.context")]
    public string? LinkedResourcesOdataContext { get; set; }

    [JsonProperty("linkedResources")]
    public List<LinkedResourceData>? LinkedResources { get; set; }

    [JsonProperty("@odata.etag")]
    public string? OdataEtag { get; set; }
}

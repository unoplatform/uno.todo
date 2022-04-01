using Newtonsoft.Json;

namespace ToDo.Data.Models
{
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
        public TaskBody? Body { get; set; }

        [JsonProperty("dueDateTime")]
        public DueDateTime? DueDateTime { get; set; }

        [JsonProperty("linkedResources@odata.context")]
        public string? LinkedResourcesOdataContext { get; set; }

        [JsonProperty("linkedResources")]
        public List<LinkedResource>? LinkedResources { get; set; }

        [JsonProperty("@odata.etag")]
        public string? OdataEtag { get; set; }
    }
}

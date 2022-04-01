using Newtonsoft.Json;

namespace ToDo.Data.Models
{
    public class LinkedResource
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("webUrl")]
        public string? WebUrl { get; set; }

        [JsonProperty("applicationName")]
        public string? ApplicationName { get; set; }

        [JsonProperty("displayName")]
        public string? DisplayName { get; set; }
    }
}

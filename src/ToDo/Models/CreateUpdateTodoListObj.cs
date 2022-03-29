using Newtonsoft.Json;

namespace ToDo.Models
{
    public record CreateUpdateTodoListObj
    {
        [JsonProperty(PropertyName = "displayName")]
        public string? DisplayName { get; set; }
    }
}

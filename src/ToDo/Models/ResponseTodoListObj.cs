using Newtonsoft.Json;

namespace ToDo.Models;

public class ResponseTodoListObj
{
    [JsonProperty(PropertyName = "@odata.context")]
    public string? OdataContext { get; set; }

    [JsonProperty(PropertyName = "value")]
    public List<TodoList>? Value { get; set; }
}

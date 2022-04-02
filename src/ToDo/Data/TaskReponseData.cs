﻿using Newtonsoft.Json;

namespace ToDo;

public class TaskReponseData<T>
{
    [JsonProperty(PropertyName = "@odata.context")]
    public string? OdataContext { get; set; }

    [JsonProperty(PropertyName = "value")]
    public List<T>? Value { get; set; }
}


using System.Text.Json.Serialization;

namespace ToDo;

public class TaskReponseData<T>
{
	[JsonPropertyName("@odata.context")]
	public string? OdataContext { get; set; }

	[JsonPropertyName("value")]
	public List<T>? Value { get; set; }
}

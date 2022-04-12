
namespace ToDo;

public record ToDoSettings
{
	public string? LastSearch { get; init; }

	public string? CachedAccessToken { get; init; }
	public DateTime? CachedAccessTokenTimeStamp { get; init; }
}

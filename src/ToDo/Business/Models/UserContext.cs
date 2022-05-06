namespace ToDo.Business.Models;

public record UserContext
{
	public string? Name { get; init; }

	public string? Email { get; init; }

	public string? AccessToken { get; init; }

	public byte[]? ProfilePicture { get; init; }
}

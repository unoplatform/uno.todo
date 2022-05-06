namespace ToDo.Configuration;

public record Auth
{
	public string? ApplicationId { get; init; }
	public string[]? Scopes { get; init; }
	public string? RedirectUri { get; init; }
	public string? KeychainSecurityGroup { get; init; }
}

namespace ToDo.Business.Services;

public class MockAuthenticationService : IAuthenticationService
{
	private string? AccessToken;

	public async Task<AuthenticationResult?> AcquireTokenAsync()
	{
		AccessToken = "Mock Value that's not empty";
		return AuthResult;
	}
	public async Task<string> GetAccessToken() => AuthResult?.AccessToken ?? string.Empty;

	private AuthenticationResult? AuthResult => string.IsNullOrWhiteSpace(AccessToken) ? default : new AuthenticationResult(AccessToken, false, string.Empty, DateTimeOffset.MaxValue, DateTimeOffset.MaxValue, string.Empty, null, string.Empty, null, Guid.Empty);

	public Task<UserContext> ReturnAuthResultContext() => throw new NotImplementedException();

	public async Task SignOutAsync()
	{
		AccessToken = null;
	}
}


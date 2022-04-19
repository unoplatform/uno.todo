


namespace ToDo.Business.Services;

public class AuthenticationService : IAuthenticationService
{
	private readonly IPublicClientApplication _pca;
	private readonly OAuthSettings _settings;
	public AuthenticationService(IOptions<OAuthSettings> settings)
	{
		_settings = settings.Value;

		var builder = PublicClientApplicationBuilder
				.Create(_settings.ApplicationId)
				.WithRedirectUri(_settings.RedirectUri)
				.WithUnoHelpers();
		if (!string.IsNullOrWhiteSpace(_settings.KeychainSecurityGroup))
		{
			builder = builder.WithIosKeychainSecurityGroup(_settings.KeychainSecurityGroup);
		}
		_pca = builder.Build();
	}

	public async Task<UserContext> ReturnAuthResultContext()
	{
		try
		{
			var authResult = await _pca.AcquireTokenInteractive(_settings.Scopes)
				.WithUnoHelpers()
				.ExecuteAsync();
			//TODO:We need to store UserContext in order to use it in the HomeViewModel
			return CreateContextFromAuthResult(authResult);
		}
		catch (MsalClientException ex)
		{
			//This is thrown when the user closes the webview before he can authenticate
			throw new MsalClientException(ex.ErrorCode, ex.Message);
		}
		catch (Exception ex)
		{
			throw new Exception(ex.Message);
		}
	}

	private UserContext CreateContextFromAuthResult(AuthenticationResult authResult)
	{
		var token = new JwtSecurityTokenHandler().ReadJwtToken(authResult.IdToken);
		return new UserContext
		{
			Name = token.Claims.First(c => c.Type.Equals("name")).Value,
			Email = token.Claims.First(c => c.Type.Equals("preferred_username")).Value,
			AccessToken = authResult.AccessToken
		};
	}
}

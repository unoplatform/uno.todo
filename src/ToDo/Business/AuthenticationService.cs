


using System.Text.Json;

namespace ToDo.Business.Services;

public class AuthenticationService : IAuthenticationService
{
	private readonly IPublicClientApplication _pca;
	private readonly string[] _scopes;
	private readonly ILogger _logger;
	public AuthenticationService(
		ILogger<AuthenticationService> logger,
		IOptions<OAuthSettings> settings)
	{
		_logger = logger;
		var authSettings = settings.Value;
		_scopes = authSettings.Scopes ?? new string[] { };

		var builder = PublicClientApplicationBuilder
				.Create(authSettings.ApplicationId)
				.WithRedirectUri(authSettings.RedirectUri)
				.WithUnoHelpers();
		if (!string.IsNullOrWhiteSpace(authSettings.KeychainSecurityGroup))
		{
			builder = builder.WithIosKeychainSecurityGroup(authSettings.KeychainSecurityGroup);
		}
		_pca = builder.Build();
	}

	public async Task<UserContext> ReturnAuthResultContext()
	{
		try
		{
			var authResult = await _pca.AcquireTokenInteractive(_scopes)
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


	public async Task<AuthenticationResult?> AcquireTokenAsync(IDispatcher dispatcher)
	{
		var authentication = await AcquireSilentTokenAsync();

		if (string.IsNullOrEmpty(authentication?.AccessToken))
		{
			authentication = await AcquireInteractiveTokenAsync(dispatcher);
		}

		return authentication;
	}

	public async Task SignOutAsync()
	{
		var accounts = await _pca.GetAccountsAsync();
		var firstAccount = accounts.FirstOrDefault();
		if (firstAccount == null)
		{
			_logger.LogInformation(
			  "Unable to find any accounts to log out of.");
			return;
		}

		await _pca.RemoveAsync(firstAccount);
		_logger.LogInformation($"Removed account: {firstAccount.Username}, user succesfully logged out.");
	}

	private ValueTask<AuthenticationResult> AcquireInteractiveTokenAsync(IDispatcher dispatcher)
	{
		return dispatcher.ExecuteAsync(async () => await _pca
		  .AcquireTokenInteractive(_scopes)
		  .WithUnoHelpers()
		  .ExecuteAsync());
	}


	async Task<AuthenticationResult?> AcquireSilentTokenAsync()
	{
		var accounts = await _pca.GetAccountsAsync();
		var firstAccount = accounts.FirstOrDefault();

		if (firstAccount == null)
		{
			_logger.LogInformation("Unable to find Account in MSAL.NET cache");
			return default;
		}

		if (accounts.Any())
		{
			_logger.LogInformation($"Number of Accounts: {accounts.Count()}");
		}

		try
		{
			_logger.LogInformation("Attempting to perform silent sign in . . .");
			_logger.LogInformation($"Authentication Scopes: {JsonSerializer.Serialize(_scopes)}");

			_logger.LogInformation($"Account Name: {firstAccount.Username}");

			return await _pca
			  .AcquireTokenSilent(_scopes, firstAccount)
			  //.WaitForRefresh(false)
			  .ExecuteAsync();
		}
		catch (MsalUiRequiredException ex)
		{
			_logger.LogWarning(ex, ex.Message);
			_logger.LogWarning(
			  "Unable to retrieve silent sign in Access Token");
		}
		catch (Exception ex)
		{
			_logger.LogWarning(ex, ex.Message);
			_logger.LogWarning("Unable to retrieve silent sign in details");
		}

		return default;
	}

	public async Task<string> GetAccessToken()
	{

		var result = await AcquireSilentTokenAsync();
		return result?.AccessToken ?? string.Empty;
	}
}



using Microsoft.Identity.Client;
using ToDo.Business.Entities;
using Uno.UI.MSAL;
using System.IdentityModel.Tokens.Jwt;

namespace ToDo.Business.Services
{
	public class AuthenticationService: IAuthenticationService
	{
		//public UserContext _userContext;
		private readonly IPublicClientApplication _pca;
		private readonly OAuthSettings _settings;
		public AuthenticationService(HostBuilderContext context)
		{
			//_settings = Microsoft.Extensions.Configuration.ConfigurationBinder.Get<OAuthSettings>(context.Configuration.GetSection("OAuthSettings"));
			_settings = new OAuthSettings
				(
					context.Configuration["OAuthSettings:ApplicationId"],
					context.Configuration["OAuthSettings:Scopes"],
					context.Configuration["OAuthSettings:RedirectUri"]
				);

			_pca = PublicClientApplicationBuilder
					.Create(_settings.ApplicationId)
					.WithRedirectUri(_settings.RedirectUri)
					.WithUnoHelpers()
#if __IOS__
                .WithIosKeychainSecurityGroup("9TB54R6A6V.com.horus.unoplatform.todoapp")
#endif
					.Build();
		}

		public async Task<string> LoginAsync()
		{
			try
			{
				var authResult = await _pca.AcquireTokenInteractive(_settings.Scopes.Split(' '))
					.WithUnoHelpers()
					.ExecuteAsync();
				//TODO:We need to store UserContext in order to use it in the HomeViewModel
				var userContext = CreateContextFromAuthResult(authResult);
				return authResult.AccessToken;
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
				Name = token.Claims.First(c => c.Type == "name").Value,
				Email = token.Claims.First(c => c.Type == "preferred_username").Value
			};
		}
	}
}

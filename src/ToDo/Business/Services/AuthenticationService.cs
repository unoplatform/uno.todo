using System.Text.Json;
using Microsoft.Identity.Client;
using ToDo.Business.Entities;
using Uno.UI.MSAL;

namespace ToDo.Business.Services
{
	public class AuthenticationService: IAuthenticationService
	{
		//public UserContext _userContext;
		private readonly IPublicClientApplication _pca;
		public AuthenticationService()
		{
			_pca = PublicClientApplicationBuilder
					.Create(OAuthSettings.ApplicationId)
					.WithRedirectUri(OAuthSettings.RedirectUri)
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
				var authResult = await _pca.AcquireTokenInteractive(OAuthSettings.Scopes.Split(' '))
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
			return ParseToken(authResult.IdToken);
		}

		UserContext ParseToken(string idToken)
		{
			var data = idToken.Split('.')[1];
			idToken = Base64UrlDecode(data);
			var deserializeData = JsonSerializer.Deserialize<UserContext>(idToken);
			if (deserializeData != null)
			{
				return deserializeData;
			}
			else
			{
				throw new ArgumentNullException("AccessToken.IdToken", "UserContext must not be a null");
			}
		}

		private string Base64UrlDecode(string s)
		{
			s = s.Replace('-', '+').Replace('_', '/');
			s = s.PadRight(s.Length + (4 - s.Length % 4) % 4, '=');
			var byteArray = Convert.FromBase64String(s);
			var decoded = Encoding.UTF8.GetString(byteArray, 0, byteArray.Count());
			return decoded;
		}
	}
}


using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
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

		public async Task<UserContext> LoginAsync()
		{
			try
			{
				var authResult = await _pca.AcquireTokenInteractive(OAuthSettings.Scopes.Split(' '))
					.WithUnoHelpers()
					.ExecuteAsync();

				return CreateContextFromAuthResult(authResult);
			}
			catch (MsalClientException)
			{
				return new UserContext() { IsLoggedOn = false };
			}
			
		}

		private UserContext CreateContextFromAuthResult(AuthenticationResult authResult)
		{
			var user = ParseToken(authResult.IdToken);
			var newContext = new UserContext
			{ 
				IsLoggedOn = true,
				AccessToken = authResult.AccessToken,
				Name = user["given_name"]?.ToString(),
				Surname = user["surname"]?.ToString(),
				DisplayName = user["name"]?.ToString(),
				UserIdentifier = user["oid"]?.ToString(),
				Country = user["country"]?.ToString(),
				EmailAddress = authResult.Account.Username,
			};

			return newContext;
		}

		JObject ParseToken(string idToken)
		{
			idToken = idToken.Split('.')[1];
			idToken = Base64UrlDecode(idToken);
			return JObject.Parse(idToken);
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


using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
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

		public async Task<UserContext> Login()
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
			// Get the piece with actual user info
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

	public class UserContext
	{
		public string? UserIdentifier { get; set; }
		public string? DisplayName { get; set; }
		public string? Name { get; set; }
		public string? Surname { get; set; }
		public string? Country { get; set; }
		public string? EmailAddress { get; set; }
		public bool IsLoggedOn { get; set; }
		public string? AccessToken { get; set; }
		//public UserContext(string userIdentifier, string displayName, string name, string surname, string country, string emailAddress, bool isLoggedOn, string accessToken)
		//{
		//	UserIdentifier = userIdentifier;
		//	DisplayName = displayName;
		//	Name = name;
		//	Surname = surname;
		//	Country = country;
		//	EmailAddress = emailAddress;
		//	IsLoggedOn = isLoggedOn;
		//	AccessToken = accessToken;
		//}
	}
}

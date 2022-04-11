
namespace ToDo
{
	public record OAuthSettings
	{
		public string ApplicationId { get; init; }
		public string Scopes { get; init; }
		public string RedirectUri { get; init; }

		public OAuthSettings(string applicationId, string scopes, string redirectUri)
		{
			ApplicationId = applicationId;
			Scopes = scopes;
			RedirectUri = redirectUri;
		}
	}
}

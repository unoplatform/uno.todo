
namespace ToDo
{
	public record OAuthSettings
	{
		public OAuthSettings(string applicationId, string scopes, string redirectUri)
		{
			ApplicationId = applicationId;
			Scopes = scopes;
			RedirectUri = redirectUri;
		}

		public string ApplicationId { get; init; }
		public string Scopes { get; init; }
		public string RedirectUri { get; init; }

	}
}

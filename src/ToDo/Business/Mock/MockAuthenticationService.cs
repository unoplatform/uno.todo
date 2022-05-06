namespace ToDo.Business.Mock;

public class MockAuthenticationService : IAuthenticationService
{
	private UserContext? _user;

	public async Task<string> GetAccessToken() => _user?.AccessToken ?? string.Empty;

	public async Task<UserContext?> GetCurrentUserAsync() => _user;

	public async Task<UserContext?> AuthenticateAsync(IDispatcher dispatcher, IUserProfilePictureService userProfilePictureService, CancellationToken cancellation)
	{
		_user = new UserContext
		{
			Name = "Foo Bar",
			Email = "foo.bar@gmail.com",
			AccessToken = "MOCK_ACCESS_TOKEN"
		};

		var profilePicture = await userProfilePictureService.GetAsync(cancellation);
		if (profilePicture != null && profilePicture.Length > 0 && _user != default)
		{
			_user = _user with { ProfilePicture = profilePicture };
		}

		return _user;
	}

	public async Task SignOutAsync()
	{
		_user = null;
	}
}

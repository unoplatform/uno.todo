namespace ToDo.Business.Mock;

public class MockAuthenticationService : IAuthenticationService
{
	private UserContext? _user;

	public async Task<string> GetAccessToken() => _user?.AccessToken ?? string.Empty;

	public async Task<UserContext?> GetCurrentUserAsync() => _user;

	public async Task<UserContext?> AuthenticateAsync(IDispatcher dispatcher)
	{
		_user = new UserContext
		{
			Name = "Foo Bar",
			Email = "foo.bar@gmail.com",
			AccessToken = "MOCK_ACCESS_TOKEN"
		};

		return _user;
	}

	public async Task SignOutAsync()
	{
		_user = null;
	}

	public void SetUserProfilePicture(byte[] picture)
	{
		if (_user != null && picture != null && picture.Length > 0)
			_user = _user with { ProfilePicture = picture };
	}
}

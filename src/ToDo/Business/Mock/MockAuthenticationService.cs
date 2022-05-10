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

	public async Task SetUserProfilePicture(IUserProfilePictureService _userSvc, CancellationToken ct)
	{
		var profilePicture = await _userSvc.GetAsync(cancellationToken: ct);
		if (_user != null && profilePicture != null && profilePicture.Length > 0)
			_user = _user with { ProfilePicture = profilePicture };
	}
}

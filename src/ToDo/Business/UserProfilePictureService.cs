namespace ToDo.Business;

public class UserProfilePictureService : IUserProfilePictureService
{
	private readonly IUserProfilePictureEndpoint _client;

	private byte[]? _profilePicture;

	public UserProfilePictureService(IUserProfilePictureEndpoint client)
	{
		_client = client;
	}

	public async ValueTask<byte[]> GetAsync(CancellationToken ct)
	{
		if (_profilePicture is null ||
			_profilePicture.Length == 0 )
		{
			var content = await _client.GetAsync(ct);

			_profilePicture = await content.ReadAsByteArrayAsync();
		}
		return _profilePicture;
	}
}

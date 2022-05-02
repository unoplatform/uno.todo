namespace ToDo;

public class UserProfilePictureService : IUserProfilePictureService
{
	private readonly IUserProfilePictureEndpoint _client;

	public UserProfilePictureService(IUserProfilePictureEndpoint client)
	{
		_client = client;
	}

	public Task<byte[]> GetAsync(CancellationToken ct)
		=> _client.GetAsync(ct);
	
}

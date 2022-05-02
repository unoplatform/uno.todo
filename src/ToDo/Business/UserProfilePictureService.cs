namespace ToDo;

public class UserProfilePictureService : IUserProfilePictureService
{
	private readonly IUserProfilePictureService _client;
	private readonly IMessenger _messenger;

	public UserProfilePictureService(IUserProfilePictureService client,
		IMessenger messenger)
	{
		_client = client;
		_messenger = messenger;
	}
	public Task<byte[]> GetAsync(CancellationToken ct)
		=> _client.GetAsync(ct);
	
}

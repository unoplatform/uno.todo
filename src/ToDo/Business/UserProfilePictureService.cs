namespace ToDo.Business;

public class UserProfilePictureService : IUserProfilePictureService
{
	private readonly IUserProfilePictureEndpoint _client;

	public UserProfilePictureService(IUserProfilePictureEndpoint client)
	{
		_client = client;
	}

	public async Task<byte[]> GetAsync(CancellationToken ct)
	{
		var content
			= await _client.GetAsync(ct);

		var response = await content.ReadAsByteArrayAsync();

		return response;
	}
}

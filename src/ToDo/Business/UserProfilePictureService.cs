namespace ToDo;

public class UserProfilePictureService : IUserProfilePictureService
{
	private readonly IUserProfilePictureEndpoint _client;

	public UserProfilePictureService(IUserProfilePictureEndpoint client)
	{
		_client = client;
	}

	public async Task<byte[]> GetAsync(CancellationToken ct)
	{
		try
		{
			var response
				= await _client.GetAsync(ct);

			return new byte[0];
		}
		catch (Exception ex)
		{

			throw ex;
		}
	}
		
	
}

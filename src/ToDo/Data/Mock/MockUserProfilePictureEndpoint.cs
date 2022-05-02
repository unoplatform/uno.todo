namespace ToDo.Data.Mock;

internal class MockUserProfilePictureEndpoint : IUserProfilePictureEndpoint
{
	private readonly MockTaskListEndpoint _listEndpoint;

	public MockUserProfilePictureEndpoint(
		ITaskListEndpoint listEndpoint)
	{
		_listEndpoint = (listEndpoint as MockTaskListEndpoint)!;
	}

	public Task<byte[]> GetAsync(CancellationToken ct)
		=> _listEndpoint.GetProfilePictureAsync(ct);
}

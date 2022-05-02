namespace ToDo.Data.Mock;

internal class MockUserProfilePictureEndpoint : IUserProfilePictureEndpoint
{
	private readonly MockTaskListEndpoint _listEndpoint;

	public MockUserProfilePictureEndpoint(
		ITaskListEndpoint listEndpoint)
	{
		_listEndpoint = (listEndpoint as MockTaskListEndpoint)!;
	}

	public Task<byte[]> Get(CancellationToken ct) => _listEndpoint.GetProfilePictureAsync(ct);
}

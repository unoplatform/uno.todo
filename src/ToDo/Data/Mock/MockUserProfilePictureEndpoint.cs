namespace ToDo.Data.Mock;

internal class MockUserProfilePictureEndpoint : IUserProfilePictureEndpoint
{
	private readonly MockTaskListEndpoint _listEndpoint;

	public MockUserProfilePictureEndpoint(
		ITaskListEndpoint listEndpoint)
	{
		_listEndpoint = (listEndpoint as MockTaskListEndpoint)!;
	}

	public async Task<HttpContent> GetAsync(CancellationToken ct)
		=> await _listEndpoint.GetProfilePictureAsync(ct);
}

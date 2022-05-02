
namespace ToDo;

public interface IUserProfilePictureService
{
	Task<byte[]> GetAsync(CancellationToken ct);
}

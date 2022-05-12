
namespace ToDo.Business;

public interface IUserProfilePictureService
{
	ValueTask<byte[]> GetAsync(CancellationToken cancellationToken);
}


namespace ToDo.Business;

public interface IUserProfilePictureService
{
	Task<byte[]> GetAsync(CancellationToken cancellationToken);
}

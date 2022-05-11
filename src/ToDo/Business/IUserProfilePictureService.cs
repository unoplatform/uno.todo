
namespace ToDo.Business;

public interface IUserProfilePictureService
{
	ValueTask<byte[]> GetAsync(string userEmailAddress, CancellationToken cancellationToken);
}

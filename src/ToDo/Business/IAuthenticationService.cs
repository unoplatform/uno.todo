namespace ToDo.Business.Services;

public interface IAuthenticationService: IAuthenticationTokenProvider
{
	Task<UserContext?> GetCurrentUserAsync();

	Task<UserContext?> AuthenticateAsync(IDispatcher dispatcher, IUserProfilePictureService userProfilePictureService, CancellationToken cancellation);

	Task SignOutAsync();
}

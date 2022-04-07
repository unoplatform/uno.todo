

namespace ToDo.Business.Services
{
	public interface IAuthenticationService
	{
		//This will be called from WelcomeViewModel in order to get access token and user's data(email,name).
		//because if the user tap on Close icon on the webview authentication screen the user should be redirected to WelcomePage
		Task<UserContext> Login();
	}
}

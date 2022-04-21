using ToDo.Business.Services;

namespace ToDo.Presentation;

public partial class WelcomeViewModel
{
	private readonly IAuthenticationService _authService;
	private readonly INavigator _navigator;

	private WelcomeViewModel(
		INavigator navigator,
		ICommandBuilder getStarted,
		IAuthenticationService authService)
    {
		_navigator=navigator;
		_authService = authService;
		getStarted.Execute(GetStarted);
	}

	private async ValueTask GetStarted(CancellationToken ct)
	{
		var token = await _authService.AcquireTokenAsync();

		if(!string.IsNullOrWhiteSpace(token?.AccessToken))
		{
			await _navigator.NavigateRouteAsync(this, string.Empty);
		}
	}
}

using ToDo.Business.Services;

namespace ToDo.Presentation;

public partial class WelcomeViewModel
{
	private readonly IAuthenticationService _authService;
	private readonly INavigator _navigator;
	private readonly IDispatcher _dispatcher;

	private WelcomeViewModel(
		IDispatcher dispatcher,
		INavigator navigator,
		ICommandBuilder getStarted,
		IAuthenticationService authService)
	{
		_dispatcher = dispatcher;
		_navigator =navigator;
		_authService = authService;

		getStarted.Execute(GetStarted);
	}

	private async ValueTask GetStarted(CancellationToken ct)
	{
		var user = await _authService.AuthenticateAsync(_dispatcher);

		if(user != null)
		{
			await _navigator.NavigateRouteAsync(this, string.Empty);
		}
	}
}

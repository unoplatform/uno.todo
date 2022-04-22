using ToDo.Business.Services;

namespace ToDo.Presentation;

public partial class SettingsViewModel
{
	private readonly IAuthenticationService _authService;
	private readonly INavigator _navigator;

	private SettingsViewModel(
		INavigator navigator,
		ICommandBuilder signOut,
		IAuthenticationService authService)
	{
		_navigator = navigator;
		_authService = authService;
		signOut.Execute(SignOut);
	}

	private async ValueTask SignOut(CancellationToken ct)
	{
		await _authService.SignOutAsync();

		await _navigator.NavigateRouteAsync(this, string.Empty);
	}
}

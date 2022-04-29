

namespace ToDo.Presentation;

public partial class SettingsViewModel
{
	private readonly IAuthenticationService _authService;
	private readonly INavigator _navigator;
	public IWritableOptions<LocalizationSettings> LocalizationSettings { get; }

	private SettingsViewModel(
		INavigator navigator,
		ICommandBuilder signOut,
		IAuthenticationService authService,
		IWritableOptions<LocalizationSettings> localizationSettings)
	{
		_navigator = navigator;
		_authService = authService;
		LocalizationSettings = localizationSettings;
		signOut.Execute(SignOut);
	}

	private async ValueTask SignOut(CancellationToken ct)
	{
		var response = await _navigator.NavigateRouteForResultAsync<DialogAction>(this, "ConfirmSignOut", qualifier: Qualifiers.Dialog, cancellation: ct);
		if (response is null)
		{
			return;
		}

		var result = await response.Result;
		if (result.SomeOrDefault()?.Id?.ToString() == "SO")
		{
			await _authService.SignOutAsync();

			await _navigator.NavigateRouteAsync(this, string.Empty);
		}
	}
}

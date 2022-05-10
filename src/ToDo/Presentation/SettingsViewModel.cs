namespace ToDo.Presentation;

public partial class SettingsViewModel
{
	private readonly IAuthenticationService _authService;
	private readonly INavigator _navigator;

	public IWritableOptions<LocalizationSettings> LocalizationSettings { get; }

	public DisplayCulture[] Cultures { get; }

	private SettingsViewModel(
		INavigator navigator,
		IAuthenticationService authService,
		IWritableOptions<LocalizationSettings> localizationSettings,
		IStringLocalizer localizer)
	{
		_navigator = navigator;
		_authService = authService;
		LocalizationSettings = localizationSettings;

		Cultures = LocalizationSettings.Value!.Cultures!.Select(c => new DisplayCulture(localizer[$"SettingsPage_LanguageLabel_{c}"], c)).ToArray();
		SelectedCulture = State.Value(this, () => Cultures.FirstOrDefault(c => c.Culture == LocalizationSettings.Value?.CurrentCulture) ?? Cultures.First());

		SelectedCulture.Execute(ChangeLanguage);
	}

	public IFeed<UserContext?> CurrentUser => Feed<UserContext?>.Async(async ct => await _authService.GetCurrentUserAsync());

	[Value]
	public IState<DisplayCulture> SelectedCulture { get; }

	public async ValueTask SignOut(CancellationToken ct)
	{
		var response = await _navigator.NavigateRouteForResultAsync<LocalizableDialogAction>(this, "ConfirmSignOut", qualifier: Qualifiers.Dialog, cancellation: ct);
		if (response is null)
		{
			return;
		}

		var result = await response.Result;
		if (result.SomeOrDefault()?.Id == DialogResults.Affirmative)
		{
			await _authService.SignOutAsync();

			await _navigator.NavigateRouteAsync(this, string.Empty, cancellation: ct);
		}
	}

	private async ValueTask ChangeLanguage(DisplayCulture? culture, CancellationToken ct)
	{
		if (culture is not null)
		{
			await LocalizationSettings.Update(settings => settings.CurrentCulture = culture.Culture);
		}
	}

	public record DisplayCulture(string Display, string Culture);
}

namespace ToDo.Presentation.Dialogs;

public partial class SettingsViewModel
{
	private readonly IAuthenticationService _authService;
	private readonly INavigator _navigator;
	private IAppTheme _appTheme;
	private IWritableOptions<ToDoApp> _appSettings;

	public IWritableOptions<LocalizationSettings> LocalizationSettings { get; }

	public DisplayCulture[] Cultures { get; }

	public string[] AppThemes { get; }


	private SettingsViewModel(
		INavigator navigator,
		IAuthenticationService authService,
		IWritableOptions<LocalizationSettings> localizationSettings,
		IStringLocalizer localizer,
		IAppTheme appTheme,
		IWritableOptions<ToDoApp> appSettings)
	{
		_navigator = navigator;
		_authService = authService;
		LocalizationSettings = localizationSettings;
		_appTheme = appTheme;
		_appSettings = appSettings;

		AppThemes = new string[] { localizer["SettingsPage_ThemeLight"], localizer["SettingsPage_ThemeDark"] };
		SelectedAppTheme = State.Value(this, () => AppThemes[appTheme.IsDark ? 1 : 0]);

		SelectedAppTheme.Execute(ChangeAppTheme);

		Cultures = LocalizationSettings.Value!.Cultures!.Select(c => new DisplayCulture(localizer[$"SettingsPage_LanguageLabel_{c}"], c)).ToArray();
		SelectedCulture = State.Value(this, () => Cultures.FirstOrDefault(c => c.Culture == LocalizationSettings.Value?.CurrentCulture) ?? Cultures.First());

		SelectedCulture.Execute(ChangeLanguage);
	}

	public IFeed<UserContext?> CurrentUser => Feed<UserContext?>.Async(async ct => await _authService.GetCurrentUserAsync());

	[Value]
	public IState<DisplayCulture> SelectedCulture { get; }

	[Value]
	public IState<string> SelectedAppTheme { get; }


	public async ValueTask SignOut(CancellationToken ct)
	{
		var result = await _navigator.NavigateRouteForResultAsync<LocalizableDialogAction>(this, Dialog.ConfirmSignOut, cancellation: ct).AsResult();
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


	private async ValueTask ChangeAppTheme(string? appTheme, CancellationToken ct)
	{
		if (appTheme is { Length: > 0 })
		{
			var isDark = Array.IndexOf(AppThemes, appTheme) == 1;
			await _appTheme.SetThemeAsync(isDark);
			await _appSettings.Update(s => s with { IsDark = isDark });
		}
	}

	public record DisplayCulture(string Display, string Culture);
}

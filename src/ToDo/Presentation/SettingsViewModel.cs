namespace ToDo.Presentation;

public partial class SettingsViewModel
{
	private readonly IAuthenticationService _authService;
	private readonly INavigator _navigator;
	private string _selectedAppTheme;
	private IAppTheme _appTheme;
	private IWritableOptions<ToDoApp> _appSettings;

	public IWritableOptions<LocalizationSettings> LocalizationSettings { get; }

	public DisplayCulture[] Cultures { get; }

	public string[] AppThemes { get; } 

	public string SelectedAppTheme
	{
		get => _selectedAppTheme;
		set
		{
			_ = DoThemeChange(value, CancellationToken.None);
		}
	}


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

		AppThemes = new string [] { localizer["SettingsPage_ThemeLight"], localizer["SettingsPage_ThemeDark"] };
		_selectedAppTheme = AppThemes[appTheme.IsDark ? 1 : 0];

		Cultures = LocalizationSettings.Value!.Cultures!.Select(c => new DisplayCulture(localizer[$"SettingsPage_LanguageLabel_{c}"], c)).ToArray();
		SelectedCulture = State.Value(this, () => Cultures.FirstOrDefault(c => c.Culture == LocalizationSettings.Value?.CurrentCulture) ?? Cultures.First());

		SelectedCulture.Execute(ChangeLanguage);
	}

	public IFeed<UserContext?> CurrentUser => Feed<UserContext?>.Async(async ct => await _authService.GetCurrentUserAsync());

	[Value]
	public IState<DisplayCulture> SelectedCulture { get; }

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

	private async ValueTask DoThemeChange(string appTheme, CancellationToken ct)
	{
		_selectedAppTheme = appTheme;

		var isDark = Array.IndexOf(AppThemes, _selectedAppTheme) == 1;
		_appTheme.SetTheme(isDark);
		await _appSettings.Update(s => s with { IsDark = isDark });
	}

	public record DisplayCulture(string Display, string Culture);
}

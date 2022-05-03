

using Microsoft.Extensions.Localization;

namespace ToDo.Presentation;

[ReactiveBindable]
public partial class SettingsViewModel
{
	private readonly IAuthenticationService _authService;
	private readonly INavigator _navigator;
	private DisplayCulture _selectedCulture;

	public IWritableOptions<LocalizationSettings> LocalizationSettings { get; }

	public DisplayCulture[] Cultures { get; }

	public DisplayCulture SelectedCulture
	{
		get => _selectedCulture;
		set {
			_selectedCulture = value;
			_ = DoChangeLanguage(_selectedCulture, CancellationToken.None);
		}
	}

	private SettingsViewModel(
		INavigator navigator,
		IAuthenticationService authService,
		IWritableOptions<LocalizationSettings> localizationSettings,
		IStringLocalizer localizer)
	{
		_navigator = navigator;
		_authService = authService;
		LocalizationSettings = localizationSettings;

		(Cultures, _selectedCulture) = GetCultures(localizer);
	}

	(DisplayCulture[] Cultures, DisplayCulture Current) GetCultures(IStringLocalizer localizer)
	{
		// LocalizationSettings is defined in: appsettings.json
		if (LocalizationSettings.Value?.Cultures is { Length: > 0} cultures)
		{
			var results = cultures
				.Select(culture => new DisplayCulture(GetLabelFor(culture), culture))
				.ToArray();
			var current =
				results.FirstOrDefault(x => x.Culture == LocalizationSettings.Value?.CurrentCulture) ??
				results.FirstOrDefault();

			return (results, current);
		}
		else
		{
			var results = new[] { new DisplayCulture(GetLabelFor("en"), "en") };

			return (results, results[0]);
		}

		string GetLabelFor(string culture) => localizer[$"SettingsPage_LanguageLabel_{culture}"];
	}

	public IState<UserContext?> CurrentUser => State<UserContext?>.Async(this, async ct => await _authService.GetCurrentUserAsync());

	public ICommand SignOut => Command.Async(DoSignOut);
	private async ValueTask DoSignOut(CancellationToken ct)
	{
		var response = await _navigator.NavigateRouteForResultAsync<DialogAction>(this, "ConfirmSignOut", qualifier: Qualifiers.Dialog, cancellation: ct);
		if (response is null)
		{
			return;
		}

		var result = await response.Result;
		if (result.SomeOrDefault()?.Id == DialogResults.Affirmative)
		{
			await _authService.SignOutAsync();

			await _navigator.NavigateRouteAsync(this, string.Empty);
		}
	}

	public ICommand ChangeLanguage => Command.Create<DisplayCulture>(b => b.Then(DoChangeLanguage));
	private async ValueTask DoChangeLanguage(DisplayCulture culture, CancellationToken ct)
	{
		_selectedCulture = culture;
		await LocalizationSettings.Update(settings => settings.CurrentCulture = culture.Culture);
	}

	public record DisplayCulture(string Display, string Culture);
}

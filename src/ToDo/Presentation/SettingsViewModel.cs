

using Microsoft.Extensions.Localization;

namespace ToDo.Presentation;

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
			_ = ChangeLanguage(_selectedCulture, CancellationToken.None);
		}
	}

	private SettingsViewModel(
		INavigator navigator,
		ICommandBuilder signOut,
		ICommandBuilder<DisplayCulture> changeLanguage,
		IAuthenticationService authService,
		IWritableOptions<LocalizationSettings> localizationSettings,
		IStringLocalizer localizer)
	{
		_navigator = navigator;
		_authService = authService;
		LocalizationSettings = localizationSettings;

		if (LocalizationSettings.Value?.Cultures?.Any() ?? false)
		{
			Cultures = LocalizationSettings.Value!.Cultures!.Select(culture => new DisplayCulture(localizer[culture], culture)).ToArray();
			_selectedCulture = (string.IsNullOrWhiteSpace(LocalizationSettings.Value?.CurrentCulture) ? Cultures[0] : Cultures.FirstOrDefault(x => x.Culture == LocalizationSettings.Value?.CurrentCulture)) ?? Cultures[0];
		}
		else
		{
			Cultures = new[] { new DisplayCulture(localizer["en"], "en") };
			_selectedCulture = Cultures[0];
		}

		changeLanguage.Execute(ChangeLanguage);
		signOut.Execute(SignOut);
	}

	public IState<UserContext?> CurrentUser => State<UserContext?>.Async(this, async ct => await _authService.GetCurrentUserAsync());

	private async ValueTask SignOut(CancellationToken ct)
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

	private async ValueTask ChangeLanguage(DisplayCulture culture, CancellationToken ct)
	{
		_selectedCulture = culture;
		await LocalizationSettings.Update(settings => settings.CurrentCulture = culture.Culture);
	}

	public record DisplayCulture(string Display, string Culture)
	{

	}
}

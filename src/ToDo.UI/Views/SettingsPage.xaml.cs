﻿namespace ToDo.Views;
using Uno.Toolkit.UI;

using System.Globalization;
using Uno.Extensions.Localization;
using Uno.Toolkit.UI;

public sealed partial class SettingsPage : Page
{
	private SettingsViewModel.BindableSettingsViewModel? ViewModel { get; set; }
	private readonly IWritableOptions<LocalizationSettings> _localizationSettings;
	private bool isInitializing;

	public SettingsPage()
	{
		this.InitializeComponent();

		DataContextChanged += (sender, contextArgs) =>
		{
			this.ViewModel = DataContext as SettingsViewModel.BindableSettingsViewModel;
			if (this.IsLoaded)
			{
				PageLoaded();
			}
		};


		this.Loaded += (s, e) =>
		{
			PageLoaded();
		};
	}
	private void PageLoaded()
	{ 
		if(ViewModel is null)
        {
			return;
        }

	isInitializing = true;

			// Set default theme
			var currentTheme = SystemThemeHelper.IsRootInDarkMode(XamlRoot) ? "Dark" : "Light";
			SelectChipGroupItem(ThemeChipGroup, x => (string)x.Tag == currentTheme);

			//Set default language
			var currentCulture = ViewModel?.LocalizationSettings.Value.CurrentCulture;
			SelectChipGroupItem(LanguageChipGroup, x => (string)x.Tag == currentCulture);

			// Set default theme
			var currentTheme = SystemThemeHelper.IsRootInDarkMode(XamlRoot) ? "Dark" : "Light";
			SelectChipGroupItem(ThemeChipGroup, x => (string)x.Tag == currentTheme);
	}

	private void UpdateAppLanguage(object sender, ChipItemEventArgs e)
	{
		if (e.Item is Chip chip)
		{
			// This requires an app restart
			ViewModel?.LocalizationSettings.Update(settings =>
			{
				settings.CurrentCulture = (string)chip.Tag;
			});
		}
	}

	private void UpdateAppColorPalette(object sender, ChipItemEventArgs e)
	{
		//if (e.Item is Chip chip && GetX((string)chip.Tag) is { } something)
		//{
		//	// do stuff with something
		//}

		//object GetX(string tag) => tag switch
		//{
		//	"Purple" => "palette1.xaml",
		//	"Blue" => "palette1.xaml",
		//	"Yellow" => "palette1.xaml",

		//	_ => throw new ArgumentOutOfRangeException(),
		//}
	}

	private void UpdateAppTheme(object sender, ChipItemEventArgs e)
	{
		if (e.Item is Chip chip)
		{
			SystemThemeHelper.SetRootTheme(XamlRoot, (string)chip.Tag == "Dark");
		}
	}

	private void SelectChipGroupItem(ChipGroup group, Func<Chip, bool> predicate)
	{
		if (group.Items.Cast<Chip>().FirstOrDefault(predicate) is { } item)
		{
			group.SelectedItem = item;
		}
	}

	private void UpdateAppTheme(object sender, ChipItemEventArgs e)
	{
		if (isInitializing) return;

		if (e.Item is Chip chip)
		{
			SystemThemeHelper.SetRootTheme(XamlRoot, (string)chip.Tag == "Dark");
		}
	}

	private void SelectChipGroupItem(ChipGroup group, Func<Chip, bool> predicate)
	{
		if (group.Items.Cast<Chip>().FirstOrDefault(predicate) is { } item)
		{
			group.SelectedItem = item;
		}
	}
}

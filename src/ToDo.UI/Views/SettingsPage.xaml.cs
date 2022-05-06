using ToDo.Helpers;
using Uno.Toolkit.UI;

using ChipControl = global::Uno.Toolkit.UI.Chip; // aliasing to prevent collision, since `global::Chip` is a namespace on ios/macos...

namespace ToDo.Views;

public sealed partial class SettingsPage : Page
{
	private SettingsViewModel.BindableSettingsViewModel? ViewModel { get; set; }

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
		if (ViewModel is null)
		{
			return;
		}

		isInitializing = true;

		// Set default theme
		var currentTheme = SystemThemeHelper.IsRootInDarkMode(XamlRoot) ? "Dark" : "Light";
		SelectChipGroupItem(ThemeChipGroup, x => (string)x.Tag == currentTheme);

		// Set default theme
		isInitializing = false;
	}

	private async void UpdateAppColorPalette(object sender, ChipItemEventArgs e)
	{
		ResourceDictionary? GetPalette(string? tag) =>
			tag?.StartsWith("ms-appx:///Styles/") == true
				? new ResourceDictionary() { Source = new Uri(tag) }
				: throw new ArgumentOutOfRangeException();

		if (e.Item is Chip chip && GetPalette(chip.Tag as string) is { } newPalette)
		{
			//await FigmaService.Instance.YieldToFigma();
			//await Task.Yield();

			// update the underlying theme resources
			var changed = false;
			if (newPalette.ThemeDictionaries is { Count: > 0 } themeDictionaries)
			{
				foreach (var x in themeDictionaries)
				{
					if (x.Value is ResourceDictionary themeDictionary && x.Key is string theme)
					{
						var c = ThemeService.ApplyThemeResources(XamlRoot, themeDictionary, theme);
						changed = changed || c;
					}
				}
			}
			else
			{
				changed = ThemeService.ApplyThemeResources(XamlRoot, newPalette);
			}

			// force app to refresh (uno:style reload, win: toggle dark/light and then toggle back)
			if (changed)
			{
				//await FigmaService.Instance.YieldToFigma();
				//await Task.Yield();
				await ThemeService.ReapplyCurrentTheme(XamlRoot);
			}
		}
	}

	private void UpdateAppTheme(object sender, ChipItemEventArgs e)
	{
		if (isInitializing) return;

		if (e.Item is ChipControl chip)
		{
			SystemThemeHelper.SetRootTheme(XamlRoot, (string)chip.Tag == "Dark");
		}
	}

	private void SelectChipGroupItem(ChipGroup group, Func<ChipControl, bool> predicate)
	{
		if (group.Items.Cast<ChipControl>().FirstOrDefault(predicate) is { } item)
		{
			group.SelectedItem = item;
		}
	}
}

using Uno.Toolkit.UI;

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

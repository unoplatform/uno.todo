namespace ToDo.Views;
using Uno.Toolkit.UI;

public sealed partial class SettingsPage : Page
{
	private bool isInitializing;

	public SettingsPage()
	{
		this.InitializeComponent();

		this.Loaded += (s, e) =>
		{
			isInitializing = true;
			
			// Set default theme
			var currentTheme = SystemThemeHelper.IsRootInDarkMode(XamlRoot) ? "Dark" : "Light";
			SelectChipGroupItem(ThemeChipGroup, x => (string)x.Tag == currentTheme);

			isInitializing = false;
		};
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

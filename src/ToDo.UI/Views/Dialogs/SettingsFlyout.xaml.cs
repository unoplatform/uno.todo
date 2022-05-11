namespace ToDo.Views.Dialogs;

public sealed partial class SettingsFlyout : Flyout
{
	public SettingsFlyout()
	{
		this.InitializeComponent();
	}

	//TODO: Adjust logic to handle the Color Palette change and move it to the VM
	// Related issue: https://github.com/unoplatform/uno.todo/issues/147
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
}

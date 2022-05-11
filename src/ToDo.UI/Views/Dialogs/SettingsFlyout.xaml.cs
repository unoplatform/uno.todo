namespace ToDo.Views.Dialogs;

public sealed partial class SettingsFlyout : Flyout
{
	public SettingsFlyout()
	{
		this.InitializeComponent();

		FlyoutControl.DataContextChanged += SettingsFlyout_DataContextChanged;
	}

	// HACK: This is required because there's a bug in extensions where we're not awaiting the task that creates the viewmodel
	// Ref: https://github.com/unoplatform/uno.extensions/pull/421
	private async void SettingsFlyout_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
	{
		if(args.NewValue is Task<object?> tsk)
		{
			FlyoutControl.DataContext = await tsk;
		}
		
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

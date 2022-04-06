namespace ToDo.Views.Dialogs;

public sealed partial class AddListDialog : ContentDialog
{
	public AddListDialog()
	{
		this.InitializeComponent();

		DataContextChanged += AddListDialog_DataContextChanged;


#if WINUI
		// Hack: required until https://github.com/unoplatform/uno.extensions/pull/350
		this.XamlRoot = (App.Current as App)!.Window!.Content.XamlRoot;
#endif
	}

	private void AddListDialog_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
	{
		// Hack: Fix for issue https://github.com/unoplatform/uno.extensions/issues/351
		if (args.NewValue is Task<object> tsk)
		{
			this.DataContext = tsk.Result;
		}

	}

}

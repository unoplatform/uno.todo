namespace ToDo.Views;

public sealed partial class HomePage : Page
{
	public HomePage()
	{
		this.InitializeComponent();
	}

	private void SelectedListChanged(object sender, NavigationViewSelectionChangedEventArgs args)
	{
		if(DataContext is HomeViewModel.BindableHomeViewModel vm &&
			args.SelectedItem is TaskList taskList)
		{
			vm.SelectedListChanged.Execute(taskList);
		}
	}
}

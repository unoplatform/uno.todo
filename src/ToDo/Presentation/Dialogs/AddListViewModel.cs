namespace ToDo.Presentation;

public partial class AddListViewModel
{
	private INavigator Navigator { get; }

	//public ICommand AddCommand { get; }

	public string? ListName { get; set; }

	public AddListViewModel(
		INavigator navigator)
	{

		Navigator = navigator;

		//AddCommand = new AsyncRelayCommand(Add);
	}

	public async Task Add()
	{
		await Navigator.NavigateBackWithResultAsync(this, data: new TaskListRequestData { DisplayName = ListName });
	}
}

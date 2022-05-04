namespace ToDo.Presentation;

public partial class AddTaskViewModel
{
	private INavigator Navigator { get; }

	//public ICommand AddCommand { get; }

	public string? Title { get; set; }

	public AddTaskViewModel(
		INavigator navigator)
	{

		Navigator = navigator;

		//AddCommand = new AsyncRelayCommand(Add);
	}

	public async Task Add()
	{
		await Navigator.NavigateBackWithResultAsync(this, data: new TaskData { Title = Title });
	}
}

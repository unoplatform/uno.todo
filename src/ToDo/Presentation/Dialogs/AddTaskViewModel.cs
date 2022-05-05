namespace ToDo.Presentation;

public partial class AddTaskViewModel
{
	private INavigator Navigator { get; }

	public string? Task { get; set; }

	public AddTaskViewModel(
		INavigator navigator)
	{
		Navigator = navigator;
	}

	public async Task Add()
	{
		await Navigator.NavigateBackWithResultAsync(this, data: new TaskData { Title = Task });
	}

}

namespace ToDo.Presentation;

public partial class AddListViewModel
{
	private INavigator Navigator { get; }

	public string? ListName { get; set; }

	public AddListViewModel(
		INavigator navigator)
	{
		Navigator = navigator;
	}

	public async Task Add()
	{
		await Navigator.NavigateBackWithResultAsync(this, data: new TaskListRequestData { DisplayName = ListName });
	}
}

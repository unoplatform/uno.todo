
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace ToDo.Presentation;

public class AddListViewModel
{
	private INavigator Navigator { get; }

	public ICommand AddCommand { get; }

	public string? ListName { get; set; }

	public AddListViewModel(
		INavigator navigator)
	{

		Navigator = navigator;

		AddCommand = new AsyncRelayCommand(Add);
	}

	public async Task Add()
	{
		await Navigator.NavigateBackWithResultAsync(this, data: new ToDoTaskListRequestData { DisplayName = ListName });
	}
}

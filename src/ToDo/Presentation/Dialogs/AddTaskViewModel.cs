
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace ToDo.Presentation;

public class AddTaskViewModel
{
	private INavigator Navigator { get; }

	public ICommand AddCommand { get; }

	public string? Title { get; set; }

	public AddTaskViewModel(
		INavigator navigator)
	{

		Navigator = navigator;

		AddCommand = new AsyncRelayCommand(Add);
	}

	public async Task Add()
	{
		await Navigator.NavigateBackWithResultAsync(this, data: new TaskData { Title = Title });
	}
}

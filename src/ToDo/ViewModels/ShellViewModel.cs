
namespace ToDo.ViewModels;

public class ShellViewModel
{
	private INavigator Navigator { get; }


	public ShellViewModel(
		INavigator navigator)
	{

		Navigator = navigator;

		_ = Start();
	}

	public async TaskService Start()
	{
		await Navigator.NavigateViewModelAsync<MainViewModel>(this);
	}
}
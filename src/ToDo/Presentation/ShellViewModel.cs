
namespace ToDo.Presentation;

public class ShellViewModel
{
	private INavigator Navigator { get; }


	public ShellViewModel(
		INavigator navigator)
	{

		Navigator = navigator;

		_ = Start();
	}

	public async Task Start()
	{
		// Change the viewmodel to specify the first page of the application to navigate to
		await Navigator.NavigateViewModelAsync<WelcomeViewModel>(this);
	}
}

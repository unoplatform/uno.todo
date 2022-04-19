
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace ToDo.Presentation;

public class AuthTokenViewModel
{
	private INavigator Navigator { get; }

	public ICommand OkCommand { get; }

	public string? AccessToken { get; set; }

	public AuthTokenViewModel(
		INavigator navigator)
	{

		Navigator = navigator;

		OkCommand = new AsyncRelayCommand(Ok);
	}

	public async Task Ok()
	{
		await Navigator.NavigateBackWithResultAsync(this, data: AccessToken);
	}
}

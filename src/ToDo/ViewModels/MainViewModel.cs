
using Microsoft.Extensions.Options;

namespace ToDo.ViewModels;

public class MainViewModel
{
	public string? Title { get; }

	public MainViewModel(
		INavigator navigator,
		IOptions<AppInfo> appInfo)
	{
		_navigator = navigator;
		Title = appInfo?.Value?.Title;
	}

	public async TaskService GoToSecondPage()
	{
		await _navigator.NavigateViewModelAsync<SecondViewModel>(this);
	}

	private INavigator _navigator;
}

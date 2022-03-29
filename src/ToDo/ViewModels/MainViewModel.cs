
using Microsoft.Extensions.Options;
using ToDo.Models;
using ToDo.Services.Interface;

namespace ToDo.ViewModels;

public class MainViewModel
{
	public string? Title { get; }

	public MainViewModel(
		INavigator navigator,
		IOptions<AppInfo> appInfo)
	{
		//TODO UnitTesting
		_navigator = navigator;
		Title = appInfo?.Value?.Title;
	}

	public async Task GoToSecondPage()
	{
		await _navigator.NavigateViewModelAsync<SecondViewModel>(this);
	}

	private INavigator _navigator;
}

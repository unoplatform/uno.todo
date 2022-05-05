using System.Collections.ObjectModel;
using ToDo.Business.Services;
using Microsoft.Extensions.Localization;

namespace ToDo.Presentation;

[ReactiveBindable]
public partial class HomeViewModel
{
	public record class UserProfile(string DisplayName, string? AvatarUrl);

	private readonly INavigator _navigator;
	private readonly IAuthenticationService _authSvc;
	private readonly IStringLocalizer _localizer;
	private readonly ITaskListService _listSvc;
	private readonly ILogger _logger;

	private HomeViewModel(
		ILogger<HomeViewModel> logger,
		INavigator navigator,
		IStringLocalizer localizer,
		IAuthenticationService authSvc,
		ITaskListService listSvc,
		IMessenger messenger)
	{
		_navigator = navigator;
		_localizer = localizer;
		_logger = logger;
		_navigator = navigator;
		_authSvc = authSvc;
		_listSvc = listSvc;

		Lists.Observe(messenger, list => list.Id);

		WellKnownLists = new TaskList[]
		{
			new(TaskList.WellknownListNames.Important, _localizer["HomePage_ImportantTaskListLabel"]),
			new(TaskList.WellknownListNames.Tasks, _localizer["HomePage_CommonTaskListLabel"]),
		};
	}

	public IState<UserContext?> CurrentUser => State<UserContext?>.Async(this, async ct => await _authSvc.GetCurrentUserAsync());

	private IListState<TaskList> Lists => ListState<TaskList>.Async(this, _listSvc.GetAllAsync);

	public TaskList[] WellKnownLists { get; }

	public IListFeed<TaskList> CustomLists => Lists.Where(list => list is { WellknownListName: null or "none" });

	public ICommand CreateTaskList => Command.Async(DoCreateTaskList);
	private async ValueTask DoCreateTaskList(CancellationToken ct)
	{
		var response = await _navigator.NavigateViewModelForResultAsync<AddListViewModel, TaskListRequestData>(this, qualifier: Qualifiers.Dialog, cancellation: ct);
		if (response is null)
		{
			return;
		}

		var result = await response.Result;

		var listName = result.SomeOrDefault()?.DisplayName;
		if (listName is not null)
		{
			await _listSvc.CreateAsync(listName, ct);
		}
	}
}

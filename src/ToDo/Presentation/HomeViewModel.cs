using System.Collections.ObjectModel;
using ToDo.Business.Services;

namespace ToDo.Presentation;

public partial class HomeViewModel
{
	public record class UserProfile(string DisplayName, string? AvatarUrl);

	private readonly INavigator _navigator;
	private readonly IAuthenticationService _authSvc;
	private readonly ITaskListService _listSvc;
	private readonly ILogger _logger;

	//TODO remove it
	private readonly IUserProfilePictureService _userProfilePictureService; 

	private HomeViewModel(
		ILogger<HomeViewModel> logger,
		INavigator navigator,
		IAuthenticationService authSvc,
		ITaskListService listSvc,
		IMessenger messenger,
		ICommandBuilder createTaskList,
		//TODO remove it
		IUserProfilePictureService userProfilePictureService)
	{
		_navigator = navigator;
		_logger = logger;
		_navigator = navigator;
		_authSvc = authSvc;
		_listSvc = listSvc;
		_userProfilePictureService = userProfilePictureService;

		createTaskList.Execute(CreateTaskList);

		Lists.Observe(messenger, list => list.Id);
	}

	public IState<UserContext?> CurrentUser => State<UserContext?>.Async(this, async ct => await _authSvc.GetCurrentUserAsync());

	private IListState<TaskList> Lists => ListState<TaskList>.Async(this, _listSvc.GetAllAsync);

	public TaskList[] WellKnownLists => new[] { TaskList.Important, TaskList.Tasks };

	public IListFeed<TaskList> CustomLists => Lists.Where(list => list is { WellknownListName: null or "none" });

	private async ValueTask CreateTaskList(CancellationToken ct)
	{
		//TODO: remove
		var res = await _userProfilePictureService.GetAsync(ct);

		var response = await _navigator.NavigateViewModelForResultAsync<AddListViewModel, TaskListRequestData>(this,qualifier: Qualifiers.Dialog, cancellation: ct);
		if(response is null)
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

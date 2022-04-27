using System.Collections.ObjectModel;

namespace ToDo.Presentation;

public partial class HomeViewModel
{
	public record class UserProfile(string DisplayName, string? AvatarUrl);

	private readonly INavigator _navigator;
	private readonly ITaskListService _listSvc;
	private readonly ILogger _logger;

	private HomeViewModel(
		ILogger<HomeViewModel> logger,
		INavigator navigator,
		ITaskListService listSvc,
		IMessenger messenger,
		ICommandBuilder createTaskList,
		ICommandBuilder<TaskListData> navigateToTaskList)
	{
		_navigator = navigator;
		_logger = logger;
		_listSvc = listSvc;

		createTaskList.Execute(CreateTaskList);
		navigateToTaskList.Execute(NavigateToTaskList);

		Lists.Observe(messenger, list => list.Id);
	}

	// todo: replace with user data from api
	public UserProfile CurrentUser { get; } = new ("Xiaoy312", default);

	// the nav-view needs a 2nd dynamic list, or it would brick the static list too
	// currently CustomList throws TypeLoadException, so we use this to make the nav-view works
	// todo: replace with CustomLists
	public ObservableCollection<string> CustomLists2 { get; } = new()
	{
		"TODO", "WTB>", "Uno", "Figma", "Themes/Toolkit",
	};

	private IListState<TaskList> Lists => ListState<TaskList>.Async(this, _listSvc.GetAllAsync);

	public TaskList[] WellKnownLists => new[] { TaskList.Important, TaskList.Tasks };

	public IListFeed<TaskList> CustomLists => Lists.Where(list => list is { WellknownListName: null or "none" });

	private async ValueTask CreateTaskList(CancellationToken ct)
	{
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

	private async ValueTask NavigateToTaskList(TaskListData list, CancellationToken ct)
	{
		// TODO: Nav - Could this be an implicit nav?
		await _navigator.NavigateViewModelAsync<TaskListViewModel>(this, data: list, cancellation: ct);
	}
}

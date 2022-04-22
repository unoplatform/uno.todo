
namespace ToDo.Presentation;

public partial class HomeViewModel:IRecipient<EntityMessage<TaskList>>
{
	private readonly INavigator _navigator;
	private readonly ITaskListService _svc;
	private readonly ILogger _logger;

	private HomeViewModel(
		ILogger<HomeViewModel> logger,
		INavigator navigator,
		ITaskListService svc,
		IMessenger messenger,
		ICommandBuilder createTaskList,
		ITaskService taskEndpoint,
		ICommandBuilder<TaskListData> navigateToTaskList)
	{
		_navigator = navigator;
		_logger = logger;
		_svc = svc;
		this.taskEndpoint = taskEndpoint;

		createTaskList.Execute(CreateTaskList);
		navigateToTaskList.Execute(NavigateToTaskList);

		messenger.Register(this);
	}

	// TODO: Feed - This should be a ListFeed / This should listen for List creation/update/deletion
	private IFeed<IImmutableList<TaskList>> Lists => Feed.Async(_svc.GetAllAsync);

	// TODO: Feed - This should a ListFeed
	public IFeed<TaskList> Important => Lists.Select(allList => allList.Single(list => list is { WellknownListName: "Important" }));

	// TODO: Feed - This should a ListFeed
	public IFeed<ImmutableList<TaskList>> CustomLists => Lists.Select(allList => allList.Where(list => list is { WellknownListName: null or "none" }).ToImmutableList());

	private async ValueTask CreateTaskList(CancellationToken ct)
	{
		var response = await _navigator.NavigateViewModelForResultAsync<AddListViewModel, TaskListRequestData>(this,qualifier: Qualifiers.Dialog, cancellation: ct);
		if(response is null)
		{
			return;
		}

		var result = await response.Result;

		var listName = result.SomeOrDefault()?.DisplayName;
		if (listName is not null) {
			// TODO: Build query parameter to create the task list
			var newTaskList = _svc.CreateAsync(listName, ct);

			// TODO: Feed - Edit the local state to add the newly created list, so the previous page is updated live.
			// await Lists.Update(lists => lists.Add(newTaskList));
		}
	}

	private async ValueTask NavigateToTaskList(TaskListData list, CancellationToken ct)
	{
		// TODO: Nav - Could this be an implicit nav?
		await _navigator.NavigateViewModelAsync<TaskListViewModel>(this, data: list, cancellation: ct);
	}

	public void Receive(EntityMessage<TaskList> msg)
	{
		try
		{
			// TODO: Feed
			//await Lists.Update(tasks =>
			//{
			//	return msg.Change switch
			//	{
			//		EntityChange.Create => tasks.Add(msg.Value),
			//		EntityChange.Update => tasks.Replace(msg.Value),
			//		EntityChange.Delete => tasks.Remove(msg.Value),
			//	};
			//});
		}
		catch (Exception e)
		{
			_logger.LogError(e,"Failed to apply update message.");
		}
	}
}

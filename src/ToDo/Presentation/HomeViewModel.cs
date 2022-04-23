
namespace ToDo.Presentation;

public partial class HomeViewModel : IRecipient<EntityMessage<TaskList>>
{
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

		messenger.Register(this);
	}

	private IListState<TaskList> Lists => ListState<TaskList>.Async(this, _listSvc.GetAllAsync);

	public IFeed<TaskList> Tasks => Lists.AsFeed().Select(lists => lists.Single(list => list is { WellknownListName: "tasks" }));

	public TaskList Important => TaskList.Important;

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

	public async void Receive(EntityMessage<TaskList> msg)
	{
		var ct = CancellationToken.None;
		try
		{
			switch (msg.Change)
			{
				case EntityChange.Create:
					await Lists.AddAsync(msg.Value, ct);
					break;

				// TODO Feed
				//EntityChange.Update => tasks.Replace(msg.Value),
				//EntityChange.Delete => tasks.Remove(msg.Value),
			}
		}
		catch (Exception e)
		{
			_logger.LogError(e,"Failed to apply update message.");
		}
	}
}

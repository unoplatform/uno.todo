namespace ToDo.Presentation;

public partial class TaskListViewModel: IRecipient<EntityMessage<ToDoTask>>
{
	private readonly INavigator _navigator;
	private readonly IToDoTaskListService _listSvc;
	private readonly IToDoTaskService _taskSvc;
	private readonly IState<ToDoTaskList> _entity;
	private readonly ILogger _logger;

	private TaskListViewModel(
		ILogger<TaskListViewModel> logger,
		INavigator navigator,
		IToDoTaskListService listSvc,
		IToDoTaskService taskSvc,
		IMessenger messenger,
		IInput<ToDoTaskList> entity,
		ICommandBuilder createTask,
		ICommandBuilder<ToDoTask> navigateToTask,
		ICommandBuilder deleteList)
	{
		_logger = logger;
		_navigator = navigator;
		_listSvc = listSvc;
		_taskSvc = taskSvc;
		_entity = entity;

		createTask.Given(entity).Execute(CreateTask);
		navigateToTask.Execute(NavigateToTask);
		deleteList.Given(entity).Execute(DeleteList);

		// TODO: Unsubscribe
		messenger.Register(this);
	}

	// TODO: Feed - This should be a ListFeed / This should listen for Task creation/update/deletion
	public IFeed<IImmutableList<ToDoTask>> Tasks => _entity.SelectAsync(async (list, ct) => list is not null ? await _listSvc.GetTasksAsync(list, ct): default);

	private async ValueTask CreateTask(ToDoTaskList list, CancellationToken ct)
	{
		// TODO: Configure properties of TaskData
		var newTask = new ToDoTask {Title = "Hello world"};
		await _taskSvc.CreateAsync(list, newTask, ct);
	}

	private async ValueTask NavigateToTask(ToDoTask task, CancellationToken ct)
	{
		// TODO: Nav - Could this be an implicit nav?
		await _navigator.NavigateViewModelAsync<TaskViewModel>(this, data: task, cancellation: ct);
	}

	private async ValueTask DeleteList(ToDoTaskList list, CancellationToken ct)
	{
		await _listSvc.DeleteAsync(list, ct);
		await _navigator.NavigateBackAsync(this, cancellation: ct);
	}

	public void Receive(EntityMessage<ToDoTask> msg)
	{
		try
		{
			// TODO: Feed
			//var listOpt = await _entity;
			//if (listOpt.IsSome(out var list) && msg.Value.ListId == list.Id)
			//{
			//	await Tasks.Update(tasks =>
			//	{
			//		return msg.Change switch
			//		{
			//			EntityChange.Create => tasks.Add(msg.Value),
			//			EntityChange.Update => tasks.Replace(msg.Value),
			//			EntityChange.Delete => tasks.Remove(msg.Value),
			//		};
			//	});
			//}
		}
		catch (Exception e)
		{
			_logger.LogError(e,"Failed to apply update message.");
		}
	}
}

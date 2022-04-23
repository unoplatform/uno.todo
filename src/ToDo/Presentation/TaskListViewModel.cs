namespace ToDo.Presentation;

public partial class TaskListViewModel : IRecipient<EntityMessage<ToDoTask>>
{
	private readonly INavigator _navigator;
	private readonly ITaskListService _listSvc;
	private readonly ITaskService _taskSvc;
	private readonly IState<TaskList> _entity;
	private readonly ILogger _logger;

	private TaskListViewModel(
		ILogger<TaskListViewModel> logger,
		INavigator navigator,
		ITaskListService listSvc,
		ITaskService taskSvc,
		IMessenger messenger,
		IInput<TaskList> entity,
		ICommandBuilder createTask,
		ICommandBuilder<ToDoTask> navigateToTask,
		ICommandBuilder deleteList,
		ICommandBuilder renameList)
	{
		_logger = logger;
		_navigator = navigator;
		_listSvc = listSvc;
		_taskSvc = taskSvc;
		_entity = entity;

		createTask.Given(entity).Then(CreateTask);
		navigateToTask.Then(NavigateToTask);
		deleteList.Given(entity).Then(DeleteList);
		renameList.Given(entity).Then(RenameList);

		// TODO: Update this to register with token = list.Id
		messenger.Register(this);
	}

	public IListState<ToDoTask> Tasks => ListState<ToDoTask>.Async(this, async ct => await (await _entity).MapAsync(_listSvc.GetTasksAsync, ct)); 

	private async ValueTask CreateTask(TaskList list, CancellationToken ct)
	{
		var response = await _navigator!.NavigateViewModelForResultAsync<AddTaskViewModel, TaskData>(this, qualifier: Qualifiers.Dialog);
		if (response is null)
		{
			return;
		}

		var result = await response.Result;

		var taskName = result.SomeOrDefault()?.Title;
		if (taskName is not null)
		{
			// TODO: Configure properties of TaskData
			var newTask = new ToDoTask { Title = taskName };
			await _taskSvc.CreateAsync(list, newTask, ct);
		}
	}

	private async ValueTask NavigateToTask(ToDoTask task, CancellationToken ct)
	{
		// TODO: Nav - Could this be an implicit nav?
		await _navigator.NavigateViewModelAsync<TaskViewModel>(this, data: task, cancellation: ct);
	}

	private async ValueTask DeleteList(TaskList list, CancellationToken ct)
	{
		var response = await _navigator!.NavigateRouteForResultAsync<DialogAction>(this, "Confirm", qualifier: Qualifiers.Dialog);
		if (response is null)
		{
			return;
		}

		var result = await response.Result;
		if (result.SomeOrDefault()?.Id?.ToString() == "Y")
		{

			await _listSvc.DeleteAsync(list, ct);
			await _navigator.NavigateBackAsync(this, cancellation: ct);
		}
	}

	private async ValueTask RenameList(TaskList list, CancellationToken ct)
	{
		var response = await _navigator!.NavigateViewModelForResultAsync<RenameListViewModel, string>(this, qualifier: Qualifiers.Dialog);
		if (response is null)
		{
			return;
		}

		var newListName = await response.Result;
		// TODO: Rename the list

	}

	public async void Receive(EntityMessage<ToDoTask> msg)
	{
		var ct = CancellationToken.None;
		try
		{
			var list = (await _entity).SomeOrDefault();
			if (list?.Id != msg.Value.ListId)
			{
				return;
			}

			switch (msg.Change)
			{
				case EntityChange.Create:
					await Tasks.AddAsync(msg.Value, ct);
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

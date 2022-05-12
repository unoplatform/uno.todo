

namespace ToDo.Presentation;

public partial class TaskListViewModel
{
	private readonly INavigator _navigator;
	private readonly ITaskListService _listSvc;
	private readonly ITaskService _taskSvc;
	private readonly ILogger _logger;

	private TaskListViewModel(
		ILogger<TaskListViewModel> logger,
		INavigator navigator,
		ITaskListService listSvc,
		ITaskService taskSvc,
		IMessenger messenger,
		TaskList entity)
	{
		_logger = logger;
		_navigator = navigator;
		_listSvc = listSvc;
		_taskSvc = taskSvc;

		Entity = State.Value(this, () => entity);

		Entity.Observe(messenger, list => list.Id);
		Tasks.Observe(messenger, Entity, (list, task) => list.Id == task.ListId, task => task.Id);
	}

	public IState<TaskList> Entity { get; }

	public IListState<ToDoTask> Tasks => ListState<ToDoTask>.Async(this, async ct => await _taskSvc.GetAllAsync((await Entity)!, ct));

	public IListFeed<ToDoTask> ActiveTasks => Tasks.Where(task => !task.IsCompleted);

	public IListFeed<ToDoTask> CompletedTasks => Tasks.Where(task => task.IsCompleted);

	public async ValueTask ToggleIsImportant(ToDoTask task, CancellationToken ct)
		=> await _taskSvc.UpdateAsync(task.WithToggledIsImportant(), ct);

	public async ValueTask ToggleIsCompleted(ToDoTask task, CancellationToken ct)
		=> await _taskSvc.UpdateAsync(task.WithToggledIsCompleted(), ct);


	public ICommand CreateTask => Command.Create(c => c.Given(Entity).Then(DoCreateTask));
	private async ValueTask DoCreateTask(TaskList list, CancellationToken ct)
	{
		var taskName = await _navigator.GetDataAsync<AddTaskViewModel, string>(this, qualifier: Qualifiers.Dialog, cancellation: ct);
		if (taskName is { Length: > 0 })
		{
			var newTask = new ToDoTask { Title = taskName };
			await _taskSvc.CreateAsync(list, newTask, ct);
		}
	}

	public ICommand DeleteList => Command.Create(c => c.Given(Entity).Then(DoDeleteList));
	private async ValueTask DoDeleteList(TaskList list, CancellationToken ct)
	{
		var result = await _navigator.NavigateRouteForResultAsync<LocalizableDialogAction>(this, Dialog.ConfirmDeleteList, cancellation: ct).AsResult();
		if (result.SomeOrDefault()?.Id == DialogResults.Affirmative)
		{
			await _listSvc.DeleteAsync(list, ct);
			await _navigator.NavigateBackAsync(this, cancellation: ct);
		}
	}

	public ICommand RenameList => Command.Create(c => c.Given(Entity).Then(DoRenameList));
	private async ValueTask DoRenameList(TaskList list, CancellationToken ct)
	{
		var response = await _navigator.NavigateViewModelForResultAsync<RenameListViewModel, string>(this, qualifier: Qualifiers.Dialog, data: list, cancellation: ct);
		if (response is null)
		{
			return;
		}

		var newListName = (await response.Result).SomeOrDefault();
		if (!string.IsNullOrWhiteSpace(newListName))
		{
			list = list with { DisplayName = newListName };
			await _listSvc.UpdateAsync(list, ct);
		}
	}
}


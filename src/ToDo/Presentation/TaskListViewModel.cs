using System.Runtime.CompilerServices;
using Uno.Extensions.Reactive.Core;

namespace ToDo.Presentation;

[ReactiveBindable]
public partial class TaskListViewModel : IRecipient<EntityMessage<ToDoTask>>
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

		Entity = State.Async(this, async _ => entity);

		messenger.Register<EntityMessage<ToDoTask>>(this);
		Entity.Observe(messenger, list => list.Id);
	}

	public IState<TaskList> Entity { get; }

	public IListState<ToDoTask> Tasks => ListState<ToDoTask>.Async(this, async ct => await (await Entity).MapAsync(_taskSvc.GetAsync, ct));

	public IListFeed<ToDoTask> ActiveTasks => Tasks.Where(task => !task.IsCompleted);

	public IListFeed<ToDoTask> CompletedTasks => Tasks.Where(task => task.IsCompleted);

	public ICommand CreateTask => Command.Create(c => c.Given(Entity).Then(DoCreateTask));
	private async ValueTask DoCreateTask(TaskList list, CancellationToken ct)
	{
		var taskName = await _navigator.GetDataAsync<AddTaskViewModel, string>(this, qualifier: Qualifiers.Dialog, cancellation: ct);
		if (taskName is not null)
		{
			// TODO: Configure properties of TaskData
			var newTask = new ToDoTask { Title = taskName };
			await _taskSvc.CreateAsync(list, newTask, ct);
		}
	}

	public ICommand ToggleIsImportant => Command.Create<ToDoTask>(c => c.Then(DoToggleIsImportant));
	private async ValueTask DoToggleIsImportant(ToDoTask task, CancellationToken ct)
	{
		if (task.Importance is null)
		{
			return;
		}
		var updatedTask = task with { Importance = task.IsImportant ? ToDoTask.TaskImportance.Normal : ToDoTask.TaskImportance.Important };

		await _taskSvc.UpdateAsync(updatedTask, ct);
	}

	public ICommand DeleteList => Command.Create(c => c.Given(Entity).Then(DoDeleteList));
	private async ValueTask DoDeleteList(TaskList list, CancellationToken ct)
	{
		var response = await _navigator.NavigateRouteForResultAsync<DialogAction>(this, "ConfirmDeleteList", qualifier: Qualifiers.Dialog, cancellation: ct);
		if (response is null)
		{
			return;
		}

		var result = await response.Result;
		if (result.SomeOrDefault()?.Id == DialogResults.Affirmative)
		{
			await _listSvc.DeleteAsync(list, ct);
			await _navigator.NavigateBackAsync(this, cancellation: ct);
		}
	}

	public ICommand ToggleIsCompleted => Command.Create<ToDoTask>(c => c.Then(DoToggleIsCompleted));
	private async ValueTask DoToggleIsCompleted(ToDoTask task, CancellationToken ct)
	{
		if (task.Status is null)
		{
			return;
		}

		var updatedTask = task with { Status = task.IsCompleted ? ToDoTask.TaskStatus.NotStarted : ToDoTask.TaskStatus.Completed };
		await _taskSvc.UpdateAsync(updatedTask, ct);
	}

	public ICommand RenameList => Command.Create(c => c.Given(Entity).Then(DoRenameList));
	private async ValueTask DoRenameList(TaskList list, CancellationToken ct)
	{
		var response = await _navigator.NavigateViewModelForResultAsync<RenameListViewModel, string>(this, qualifier: Qualifiers.Dialog, cancellation: ct);
		if (response is null)
		{
			return;
		}

		var newListName = (await response.Result).SomeOrDefault();
		if (!String.IsNullOrWhiteSpace(newListName))
		{
			list = list with { DisplayName = newListName };
			await _listSvc.UpdateAsync(list, ct);
		}
	}

	public async void Receive(EntityMessage<ToDoTask> msg)
	{
		var ct = CancellationToken.None;
		try
		{
			using var _ = SourceContext.GetOrCreate(this).AsCurrent();

			var list = (await Entity).SomeOrDefault();
			if (list?.Id == msg.Value.ListId)
			{
				await Tasks.UpdateAsync(msg, task => task.Id, ct);
			}
		}
		catch (Exception e)
		{
			_logger.LogError(e,"Failed to apply task update message.");
		}
	}
}


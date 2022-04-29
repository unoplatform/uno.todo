using System.Runtime.CompilerServices;
using Uno.Extensions.Reactive.Core;

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
		ICommandBuilder<ToDoTask> starTask,
		ICommandBuilder<ToDoTask> navigateToTask,
		ICommandBuilder<ToDoTask> toggleIsCompleted,
		ICommandBuilder deleteList,
		ICommandBuilder renameList)
	{
		_logger = logger;
		_navigator = navigator;
		_listSvc = listSvc;
		_taskSvc = taskSvc;
		_entity = entity;

		createTask.Given(entity).Then(CreateTask);
		starTask.Then(StarTask);
		navigateToTask.Then(NavigateToTask);
		toggleIsCompleted.Then(ToggleIsCompleted);
		deleteList.Given(entity).Then(DeleteList);
		renameList.Given(entity).Then(RenameList);

		messenger.Register<EntityMessage<ToDoTask>>(this);
		entity.Observe(messenger, list => list.Id);
	}

	public IListState<ToDoTask> Tasks => ListState<ToDoTask>.Async(this, async ct => await (await _entity).MapAsync(_taskSvc.GetAsync, ct));

	public IListFeed<ToDoTask> ActiveTasks => Tasks.Where(task => !task.IsCompleted);

	public IListFeed<ToDoTask> CompletedTasks => Tasks.Where(task => task.IsCompleted);

	private async ValueTask CreateTask(TaskList list, CancellationToken ct)
	{
		var response = await _navigator.NavigateViewModelForResultAsync<AddTaskViewModel, TaskData>(this, qualifier: Qualifiers.Dialog, cancellation: ct);
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

	private async ValueTask StarTask(ToDoTask task, CancellationToken ct)
	{
		await Task.CompletedTask;
	}

	private async ValueTask NavigateToTask(ToDoTask task, CancellationToken ct)
	{
		// TODO: Nav - Could this be an implicit nav?
		await _navigator.NavigateViewModelAsync<TaskViewModel>(this, data: task, cancellation: ct);
	}

	private async ValueTask DeleteList(TaskList list, CancellationToken ct)
	{
		var response = await _navigator.NavigateRouteForResultAsync<DialogAction>(this, "ConfirmDeleteList", qualifier: Qualifiers.Dialog, cancellation: ct);
		if (response is null)
		{
			return;
		}

		var result = await response.Result;
		if (result.SomeOrDefault()?.Id?.ToString() == "DL")
		{

			await _listSvc.DeleteAsync(list, ct);
			await _navigator.NavigateBackAsync(this, cancellation: ct);
		}
	}
	
	private async ValueTask ToggleIsCompleted(ToDoTask task, CancellationToken ct)
	{
		if (task.Status is null)
		{
			return;
		}

		var updatedTask = task with { Status = task.IsCompleted ? ToDoTask.TaskStatus.NotStarted : ToDoTask.TaskStatus.Completed };
		await _taskSvc.UpdateAsync(updatedTask, ct);
	}

	private async ValueTask RenameList(TaskList list, CancellationToken ct)
	{
		var response = await _navigator.NavigateViewModelForResultAsync<RenameListViewModel, string>(this, qualifier: Qualifiers.Dialog, cancellation: ct);
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
			using var _ = SourceContext.GetOrCreate(this).AsCurrent();

			var list = (await _entity).SomeOrDefault();
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


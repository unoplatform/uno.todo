using System;
using System.Collections.Immutable;
using System.Linq;
using ToDo.Business;
using Uno.Extensions.Reactive;

namespace ToDo.Presentation;

public partial class TaskListViewModel
{
	private readonly INavigator _navigator;
	private readonly IToDoTaskListService _listSvc;
	private readonly IToDoTaskService _taskSvc;
	private readonly IState<ToDoTaskList> _entity;

	private TaskListViewModel(
		INavigator navigator,
		IToDoTaskListService listSvc,
		IToDoTaskService taskSvc,
		IInput<ToDoTaskList> entity,
		ICommandBuilder createTask,
		ICommandBuilder<ToDoTask> navigateToTask,
		ICommandBuilder deleteList)
	{
		_navigator = navigator;
		_listSvc = listSvc;
		_taskSvc = taskSvc;
		_entity = entity;

		createTask.Given(entity).Execute(CreateTask);
		navigateToTask.Execute(NavigateToTask);
		deleteList.Given(entity).Execute(DeleteList);
	}

	// TODO: Feed - This should be a ListFeed / This should listen for Task creation/update/deletion
	public IFeed<IImmutableList<ToDoTask>> Tasks => _entity.SelectAsync(async (list, ct) => await _listSvc.GetTasksAsync(list!, ct));

	private async ValueTask CreateTask(ToDoTaskList list, CancellationToken ct)
	{
		// TODO: Configure properties of TaskData
		var newTask = new ToDoTask {Title = "Hello world"};
		var createdTask = await _taskSvc.CreateAsync(list, newTask, ct);

		// TODO: Broadcast - Notify task created
	}

	private async ValueTask NavigateToTask(ToDoTask task, CancellationToken ct)
	{
		// TODO: Nav - Could this be an implicit nav?
		await _navigator.NavigateViewModelAsync<TaskViewModel>(this, data: task, cancellation: ct);
	}

	private async ValueTask DeleteList(ToDoTaskList list, CancellationToken ct)
	{
		await _listSvc.DeleteAsync(list, ct);

		// TODO: Broadcast - Notify list deleted

		await _navigator.NavigateBackAsync(this, cancellation: ct);
	}
}

using System;
using System.Linq;
using Uno.Extensions.Reactive;

namespace ToDo.ViewModels;

internal partial class TaskListViewModel
{
	private readonly INavigator _navigator;
	private readonly ITaskListEndpoint _listClient;
	private readonly ITaskEndpoint _taskClient;
	private readonly IFeed<TaskListData[]> _allTasksLists;
	private readonly IState<TaskListData> _entity;

	private TaskListViewModel(
		INavigator navigator,
		ITaskListEndpoint listClient,
		ITaskEndpoint taskClient,
		IFeed<TaskListData[]> allTasksLists,
		IInput<TaskListData> entity, // TODO: Feed - this should be a state
		ICommandBuilder createTask,
		ICommandBuilder<TaskData> navigateToTask,
		ICommandBuilder deleteList)
	{
		_navigator = navigator;
		_listClient = listClient;
		_taskClient = taskClient;
		_allTasksLists = allTasksLists;
		_entity = entity;

		createTask.Given(entity).Execute(CreateTask);
		navigateToTask.Execute(NavigateToTask);
		deleteList.Given(entity).Execute(DeleteList);
	}

	private async ValueTask CreateTask(TaskListData list, CancellationToken ct)
	{
		// TODO: Configure properties of TaskData
		var newTask = new TaskData();
		var createdTask = await _taskClient.CreateAsync(list.Id!, newTask, ct);
	}

	private async ValueTask NavigateToTask(TaskData theTask, CancellationToken ct)
	{
		// TODO: Propagate the ListId within the TaskData
		var list = (await _entity).SomeOrDefault()!;

		// TODO: Split data in multiple values so they can be used in the TaskViewModel ctor (especially the IInput<TaskData> entity).
		await _navigator.NavigateViewModelAsync<TaskViewModel>(this, data: (list, _entity, theTask), cancellation: ct);
	}

	private async ValueTask DeleteList(TaskListData list, CancellationToken ct)
	{
		await _listClient.DeleteAsync(list.Id!, ct);

		// TODO: Feed - Update the local state, so the previous page is updated live
		//await _allTasksLists.Update(lists => lists.Remove(list));

		await _navigator.NavigateBackAsync(this, cancellation: ct);
	}
}
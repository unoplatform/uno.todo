using System;
using System.Collections.Generic;
using System.Text;
using Uno.Extensions.Reactive;

namespace ToDo.ViewModels;

internal partial class TaskListsViewModel
{
	private readonly INavigator _navigator;
	private readonly ITaskListEndpoint _client;

	private TaskListsViewModel(
		INavigator navigator,
		ITaskListEndpoint client,
		ICommandBuilder createTaskList,
		ICommandBuilder<TaskListData> navigateToTaskList)
	{
		_navigator = navigator;
		_client = client;

		createTaskList.Execute(CreateTaskList);
		navigateToTaskList.Execute(NavigateToTaskList);
	}

	private IFeed<TaskListData[]> Lists => Feed.Async(async ct => (await _client.GetAllAsync(ct)).Value?.ToArray() ?? Array.Empty<TaskListData>());

	public IFeed<TaskListData> Important => Lists.Select(allList => allList.Single(list => list is { WellknownListName: "Important" }));

	public IFeed<TaskListData[]> CustomLists => Lists.Select(allList => allList.Where(list => list is { WellknownListName: null }).ToArray());

	private async ValueTask CreateTaskList(CancellationToken ct)
	{
		// TODO: Build query parameter to create the task list
		// var newTaskList = _client.CreateAsync(default!, ct);

		// TODO: Feed - Edit the local state to add the newly created list, so the previous page is updated live.
		// await Lists.Update(lists => lists.Add(newTaskList));
	}

	private async ValueTask NavigateToTaskList(TaskListData taskList, CancellationToken ct)
	{
		await _navigator.NavigateViewModelAsync<SecondViewModel>(this, data: (Lists, taskList), cancellation: ct);
	}
}
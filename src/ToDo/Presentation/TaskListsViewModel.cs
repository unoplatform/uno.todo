using System;
using System.Collections.Immutable;
using ToDo.Business;
using Uno.Extensions.Reactive;

namespace ToDo.Presentation;

public partial class TaskListsViewModel
{
	private readonly INavigator _navigator;
	private readonly IToDoTaskListService _svc;

	private TaskListsViewModel(
		INavigator navigator,
		IToDoTaskListService svc,
		ICommandBuilder createTaskList,
		ICommandBuilder<ToDoTaskListData> navigateToTaskList)
	{
		_navigator = navigator;
		_svc = svc;

		createTaskList.Execute(CreateTaskList);
		navigateToTaskList.Execute(NavigateToTaskList);
	}

	// TODO: Feed - This should be a ListFeed / This should listen for List creation/update/deletion
	private IFeed<IImmutableList<ToDoTaskList>> Lists => Feed.Async(_svc.GetAllAsync);

	// TODO: Feed - This should a ListFeed
	public IFeed<ToDoTaskList> Important => Lists.Select(allList => allList.Single(list => list is { WellknownListName: "Important" }));

	// TODO: Feed - This should a ListFeed
	public IFeed<ImmutableList<ToDoTaskList>> CustomLists => Lists.Select(allList => allList.Where(list => list is { WellknownListName: null or "none" }).ToImmutableList());

	private async ValueTask CreateTaskList(CancellationToken ct)
	{
		// TODO: Build query parameter to create the task list
		// var newTaskList = _client.CreateAsync(default!, ct);

		// TODO: Feed - Edit the local state to add the newly created list, so the previous page is updated live.
		// await Lists.Update(lists => lists.Add(newTaskList));
	}

	private async ValueTask NavigateToTaskList(ToDoTaskListData list, CancellationToken ct)
	{
		// TODO: Nav - Could this be an implicit nav?
		await _navigator.NavigateViewModelAsync<TaskListViewModel>(this, data: list, cancellation: ct);
	}
}

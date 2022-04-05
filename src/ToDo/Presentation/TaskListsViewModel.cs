using System;
using System.Collections.Immutable;
using ToDo.Business;
using Uno.Extensions.Reactive;
using Uno.Logging;

namespace ToDo.Presentation;

public partial class TaskListsViewModel
{
	private readonly INavigator _navigator;
	private readonly IToDoTaskListService _svc;

	private TaskListsViewModel(
		INavigator navigator,
		IToDoTaskListService svc,
		IMessenger messenger,
		ICommandBuilder createTaskList,
		ICommandBuilder<ToDoTaskListData> navigateToTaskList)
	{
		_navigator = navigator;
		_svc = svc;

		createTaskList.Execute(CreateTaskList);
		navigateToTaskList.Execute(NavigateToTaskList);

		// TODO: Unsubscribe
		messenger.TaskListChanged += OnTaskListChanged;
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

	private async void OnTaskListChanged(object sender, EntityMessage<ToDoTaskList> msg)
	{
		try
		{
			// TODO: Feed
			//await Lists.Update(tasks =>
			//{
			//	return msg.Change switch
			//	{
			//		EntityChange.Create => tasks.Add(msg.Value),
			//		EntityChange.Update => tasks.Replace(msg.Value),
			//		EntityChange.Delete => tasks.Remove(msg.Value),
			//	};
			//});
		}
		catch (Exception e)
		{
			this.Log().Error("Failed to apply update message.", e);
		}
	}
}

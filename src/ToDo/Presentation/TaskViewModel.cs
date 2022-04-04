using System;
using System.Linq;
using Uno.Extensions.Reactive;

namespace ToDo.ViewModels;

internal partial class TaskViewModel
{
	private readonly INavigator _navigator;
	private readonly ITaskEndpoint _client;
	private readonly TaskListData _theListRequiredOnlyForListIdWhichShouldBePartOfTheTaskRecord;
	private readonly IInput<TaskData> _entity;
	private readonly IFeed<TaskData[]> _allTasks;

	private TaskViewModel(
		INavigator navigator,
		ITaskEndpoint client,
		TaskListData theListRequiredOnlyForListIdWhichShouldBePartOfTheTaskRecord, 
		IFeed<TaskData[]> allTasks,
		IInput<TaskData> entity,
		ICommandBuilder delete,
		ICommandBuilder save)
	{
		_navigator = navigator;
		_client = client;
		_theListRequiredOnlyForListIdWhichShouldBePartOfTheTaskRecord = theListRequiredOnlyForListIdWhichShouldBePartOfTheTaskRecord;
		_allTasks = allTasks;
		_entity = entity;

		delete.Given(_entity).Execute(Delete);
		save.Given(_entity).Execute(Save);
	}

	private async ValueTask Delete(TaskData task, CancellationToken ct)
	{
		await _client.DeleteAsync(_theListRequiredOnlyForListIdWhichShouldBePartOfTheTaskRecord.Id!, task.Id!, ct);

		// TODO: Feed - Delete the task from the list, so the previous page is updated live
		// await _allTasks.Update(tasks => tasks.Remove(task));

		await _navigator.NavigateBackAsync(this, cancellation: ct);
	}

	private async ValueTask Save(TaskData task, CancellationToken ct)
	{
		await _client.UpdateAsync(_theListRequiredOnlyForListIdWhichShouldBePartOfTheTaskRecord.Id!, task.Id!, task, ct);
	}
}
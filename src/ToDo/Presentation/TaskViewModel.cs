using System;
using System.Linq;
using ToDo.Business;
using Uno.Extensions.Reactive;

namespace ToDo.Presentation;

public partial class TaskViewModel
{
	private readonly INavigator _navigator;
	private readonly IToDoTaskService _svc;

	private TaskViewModel(
		INavigator navigator,
		IToDoTaskService svc,
		IInput<ToDoTask> entity,
		ICommandBuilder delete,
		ICommandBuilder save)
	{
		_navigator = navigator;
		_svc = svc;

		delete.Given(entity).Execute(Delete);
		save.Given(entity).Execute(Save);
	}

	private async ValueTask Delete(ToDoTask task, CancellationToken ct)
	{
		await _svc.DeleteAsync(task, ct);
		await _navigator.NavigateBackAsync(this, cancellation: ct);
	}

	private async ValueTask Save(ToDoTask task, CancellationToken ct)
		=> await _svc.UpdateAsync(task, ct);
}

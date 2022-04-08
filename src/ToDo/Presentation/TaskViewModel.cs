namespace ToDo.Presentation;

public partial class TaskViewModel
{
	private readonly INavigator _navigator;
	private readonly ITaskService _svc;

	private TaskViewModel(
		INavigator navigator,
		ITaskService svc,
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
		var response = await _navigator!.NavigateRouteForResultAsync<DialogAction>(this, "Confirm", qualifier: Qualifiers.Dialog);
		if (response is null)
		{
			return;
		}

		var result = await response.Result;
		if (result.SomeOrDefault()?.Id?.ToString() == "Y")
		{
			await _svc.DeleteAsync(task, ct);
			await _navigator.NavigateBackAsync(this, cancellation: ct);
		}
	}

	private async ValueTask Save(ToDoTask task, CancellationToken ct)
		=> await _svc.UpdateAsync(task, ct);
}

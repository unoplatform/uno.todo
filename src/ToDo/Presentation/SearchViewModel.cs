namespace ToDo.Presentation;

public partial class SearchViewModel
{
	private readonly ITaskService _svc;
	private readonly IInput<string> _term;

	public SearchViewModel(
		ITaskService svc,
		IInput<string> term,
		ICommandBuilder<ToDoTask> toggleIsCompleted,
		ICommandBuilder<ToDoTask> toggleIsImportant)
	{
		_svc = svc;
		_term = term;

		toggleIsCompleted.Then(ToggleIsCompleted);
		toggleIsImportant.Then(ToggleIsImportant);
	}

	public IListFeed<ToDoTask> Results => _term
		.Where(term => term is { Length: > 0 })
		.SelectAsync(_svc.GetAllAsync)
		.AsListFeed();

	private async ValueTask ToggleIsCompleted(ToDoTask task, CancellationToken ct)
	{
		if (task.Status is null)
		{
			return;
		}

		var updatedTask = task with { Status = task.IsCompleted ? ToDoTask.TaskStatus.NotStarted : ToDoTask.TaskStatus.Completed };
		await _svc.UpdateAsync(updatedTask, ct);
	}

	private async ValueTask ToggleIsImportant(ToDoTask task, CancellationToken ct)
	{
		if (task.Importance is null)
		{
			return;
		}
		var updatedTask = task with { Importance = task.IsImportant ? ToDoTask.TaskImportance.Normal : ToDoTask.TaskImportance.Important };

		await _svc.UpdateAsync(updatedTask, ct);
	}
}

namespace ToDo.Presentation;

[ReactiveBindable]
public partial class SearchViewModel
{
	private readonly ITaskService _svc;

	public SearchViewModel(ITaskService svc)
	{
		_svc = svc;
	}

	public IState<string> Term => State.Async(this, async _ => "");

	public IListFeed<ToDoTask> Results => Term
		.Where(term => term is { Length: > 0 })
		.SelectAsync(_svc.GetAllAsync)
		.AsListFeed();

	public ICommand ToggleIsCompleted => Command.Create<ToDoTask>(c => c.Then(DoToggleIsImportant));
	private async ValueTask DoToggleIsCompleted(ToDoTask task, CancellationToken ct)
	{
		if (task.Status is null)
		{
			return;
		}

		var updatedTask = task with { Status = task.IsCompleted ? ToDoTask.TaskStatus.NotStarted : ToDoTask.TaskStatus.Completed };
		await _svc.UpdateAsync(updatedTask, ct);
	}

	public ICommand ToggleIsImportant => Command.Create<ToDoTask>(c => c.Then(DoToggleIsImportant));
	private async ValueTask DoToggleIsImportant(ToDoTask task, CancellationToken ct)
	{
		if (task.Importance is null)
		{
			return;
		}
		var updatedTask = task with { Importance = task.IsImportant ? ToDoTask.TaskImportance.Normal : ToDoTask.TaskImportance.Important };

		await _svc.UpdateAsync(updatedTask, ct);
	}
}

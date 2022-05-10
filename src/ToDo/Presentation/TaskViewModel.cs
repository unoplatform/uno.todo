namespace ToDo.Presentation;

public partial class TaskViewModel
{
	private readonly INavigator _navigator;
	private readonly ITaskService _svc;

	private TaskViewModel(
		INavigator navigator,
		ITaskService svc,
		ToDoTask entity)
	{
		_navigator = navigator;
		_svc = svc;

		Entity = State.Value(this, () => entity);
		Entity.Execute(async (task, ct) =>
		{
			if (task is not null)
			{
				await _svc.UpdateAsync(task, ct);
			}
		});
	}

	public IState<ToDoTask> Entity { get; }

	public ICommand ToggleIsCompleted => Command.Create(b => b.Given(Entity).Then(DoToggleIsCompleted));
	private async ValueTask DoToggleIsCompleted(ToDoTask task, CancellationToken ct)
		=> await _svc.UpdateAsync(task.WithToggledIsCompleted(), ct);

	public ICommand ToggleIsImportant => Command.Create(b => b.Given(Entity).Then(DoToggleIsImportant));
	private async ValueTask DoToggleIsImportant(ToDoTask task, CancellationToken ct)
		=> await _svc.UpdateAsync(task.WithToggledIsImportant(), ct);

	public ICommand Delete => Command.Create(b => b.Given(Entity).Then(DoDelete));
	private async ValueTask DoDelete(ToDoTask task, CancellationToken ct)
	{
		var result = await _navigator.NavigateRouteForResultAsync<LocalizableDialogAction>(this, Dialog.ConfirmDeleteTask, cancellation: ct).AsResult();
		if (result.SomeOrDefault()?.Id == DialogResults.Affirmative)
		{
			await _svc.DeleteAsync(task, ct);
			await _navigator.NavigateBackAsync(this, cancellation: ct);
		}
	}
}

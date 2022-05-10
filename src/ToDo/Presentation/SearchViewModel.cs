namespace ToDo.Presentation;

public partial class SearchViewModel
{
	private readonly ITaskService _svc;

	public SearchViewModel(ITaskService svc)
	{
		_svc = svc;
	}

	public IState<string> Term => State<string>.Empty(this);

	public IListFeed<ToDoTask> Results => Term
		.Where(term => term is { Length: > 0 })
		.SelectAsync(_svc.SearchAsync)
		.AsListFeed();

	public async ValueTask ToggleIsCompleted(ToDoTask task, CancellationToken ct)
		=> await _svc.UpdateAsync(task.WithToggledIsCompleted(), ct);

	public async ValueTask ToggleIsImportant(ToDoTask task, CancellationToken ct)
		=> await _svc.UpdateAsync(task.WithToggledIsImportant(), ct);
}

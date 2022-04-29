namespace ToDo.Presentation;

public partial class SearchViewModel
{
	private readonly ITaskService _svc;
	private readonly IInput<string> _term;

	public SearchViewModel(
		ITaskService svc,
		IInput<string> term)
	{
		_svc = svc;
		_term = term;
	}

	public IListFeed<ToDoTask> Results => _term.SelectAsync(Search).AsListFeed();

	private ValueTask<IImmutableList<ToDoTask>> Search(string s, CancellationToken ct) =>
		string.IsNullOrWhiteSpace(s)
			? new ValueTask<IImmutableList<ToDoTask>>(ImmutableList<ToDoTask>.Empty)
			: _svc.GetAllAsync(s, ct);
}

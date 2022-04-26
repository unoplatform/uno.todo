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

	public IListFeed<ToDoTask> Results => _term.SelectAsync(_svc.GetAllAsync).AsListFeed();
}

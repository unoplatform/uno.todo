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

	public IFeed<IImmutableList<ToDoTask>> AllTasks => Feed.Async(async ct => await _svc.GetAllAsync(string.Empty, ct));

	public IFeed<IImmutableList<ToDoTask>> Results => Feed.Combine(_term, AllTasks).Select(Filter);

	private static IImmutableList<ToDoTask> Filter((string term, IImmutableList<ToDoTask> tasks) inputs)
		=> inputs
			.tasks
			.Where(task => !string.IsNullOrWhiteSpace(inputs.term) && task.Body?.Content?.IndexOf(inputs.term, StringComparison.OrdinalIgnoreCase) is >= 0)
			.ToImmutableList();
}

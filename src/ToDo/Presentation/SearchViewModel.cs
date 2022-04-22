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

	public IFeed<IEnumerable<ToDoTask>> AllTasks => Feed.Async(async ct => await _svc.GetAllAsync(string.Empty, ct));

	public IFeed<IEnumerable<ToDoTask>> Results => Feed.Combine(_term, AllTasks).Select(Filter);

	private static IEnumerable<ToDoTask> Filter((string term, IEnumerable<ToDoTask> tasks) inputs)
		=> inputs
			.tasks
			.Where(task => task.Body?.Content?.IndexOf(inputs.term, StringComparison.OrdinalIgnoreCase) is >= 0)
			.ToList();
}

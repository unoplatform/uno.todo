using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using ToDo.Business;
using Uno.Extensions.Reactive;

namespace ToDo.Presentation;

public partial class SearchViewModel
{
	private readonly ITaskListService _svc;
	private readonly IInput<string> _term;

	public SearchViewModel(
		ITaskListService svc,
		IInput<string> term)
	{
		_svc = svc;
		_term = term;
	}

	private IFeed<IImmutableList<ToDoTask>> AllTasks => Feed.Async(async ct => await _svc.GetAllTasksAsync(ct));

	public IFeed<IImmutableList<ToDoTask>> Results => Feed.Combine(_term, AllTasks).Select(Filter);

	private static IImmutableList<ToDoTask> Filter((string term, IImmutableList<ToDoTask> tasks) inputs)
		=> inputs
			.tasks
			.Where(task => task.Body?.Content?.IndexOf(inputs.term, StringComparison.OrdinalIgnoreCase) is >= 0)
			.ToImmutableList();
}

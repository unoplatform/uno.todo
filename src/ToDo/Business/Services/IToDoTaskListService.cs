using System;
using System.Collections.Immutable;
using System.Linq;

namespace ToDo.Business;

public interface IToDoTaskListService
{
	ValueTask<IImmutableList<ToDoTaskList>> GetAllAsync(CancellationToken ct);

	Task<ToDoTaskList> GetAsync(string listId, CancellationToken ct);

	Task CreateAsync(string displayName, CancellationToken ct);

	Task UpdateAsync(ToDoTaskList list, CancellationToken ct);

	Task DeleteAsync(ToDoTaskList list, CancellationToken ct);

	Task<IImmutableList<ToDoTask>> GetTasksAsync(string listId, CancellationToken ct);
}

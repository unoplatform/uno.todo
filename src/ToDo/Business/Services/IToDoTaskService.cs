using System;
using System.Linq;

namespace ToDo.Business;

public interface IToDoTaskService
{
	Task<ToDoTask> GetAsync(string listId, string taskId, CancellationToken ct);

	Task<ToDoTask> CreateAsync(ToDoTaskList list, ToDoTask newTask, CancellationToken ct);

	Task<ToDoTask> UpdateAsync(ToDoTask updatedTask, CancellationToken ct);

	Task DeleteAsync(ToDoTask task, CancellationToken ct);
}

namespace ToDo.Business;

public interface IToDoTaskService
{
	Task<ToDoTask> GetAsync(string listId, string taskId, CancellationToken ct);

	Task CreateAsync(ToDoTaskList list, ToDoTask newTask, CancellationToken ct);

	Task UpdateAsync(ToDoTask task, CancellationToken ct);

	Task DeleteAsync(ToDoTask task, CancellationToken ct);
}

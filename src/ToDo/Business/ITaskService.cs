namespace ToDo.Business;

public interface ITaskService
{
	Task<ToDoTask> GetAsync(string listId, string taskId, CancellationToken ct);

	Task CreateAsync(TaskList list, ToDoTask newTask, CancellationToken ct);

	Task UpdateAsync(ToDoTask task, CancellationToken ct);

	Task DeleteAsync(ToDoTask task, CancellationToken ct);

	ValueTask<IImmutableList<ToDoTask>> GetAsync(TaskList list, CancellationToken ct);

	ValueTask<IImmutableList<ToDoTask>> GetAllAsync(string displayName = "", CancellationToken ct = default);

	ValueTask AddStepAsync(string baseTaskListId, string baseTaskId, CheckListItem checkListItem, CancellationToken ct);

	ValueTask<IImmutableList<CheckListItem>> GetStepsAsync(string baseTaskListId, string baseTaskId, CancellationToken ct);

	ValueTask DeleteStepAsync(string baseTaskListId, string baseTaskId, string checklistItemId, CancellationToken ct);

	ValueTask UpdateStepAsync(string baseTaskListId, string baseTaskId, CheckListItem checkListItem, CancellationToken ct);

}

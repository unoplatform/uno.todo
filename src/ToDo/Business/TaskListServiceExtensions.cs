namespace ToDo.Business;

public static class TaskListServiceExtensions
{
	// We cannot use default interface implementation on netstandard 2.0

	public static Task<IImmutableList<ToDoTask>> GetTasksAsync(this ITaskListService svc, TaskList list, CancellationToken ct)
		=> svc.GetTasksAsync(list.Id, ct);
}

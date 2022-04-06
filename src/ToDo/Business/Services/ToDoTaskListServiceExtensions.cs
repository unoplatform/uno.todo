namespace ToDo.Business;

public static class ToDoTaskListServiceExtensions
{
	// We cannot use default interface implementation on netstandard 2.0

	public static Task<IImmutableList<ToDoTask>> GetTasksAsync(this IToDoTaskListService svc, ToDoTaskList list, CancellationToken ct)
		=> svc.GetTasksAsync(list.Id, ct);
}

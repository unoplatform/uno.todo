namespace ToDo.Business;

public static class TaskListServiceExtensions
{
	// We cannot use default interface implementation on netstandard 2.0

	public static Task<IImmutableList<ToDoTask>> GetTasksAsync(this ITaskListService svc, TaskList list, CancellationToken ct)
		=> svc.GetTasksAsync(list.Id, ct);

	public static async Task<IImmutableList<ToDoTask>> GetAllTasksAsync(this ITaskListService svc, CancellationToken ct)
		=> await svc.GetTasksAsync(await svc.GetAllAsync(ct), ct);

	public static async Task<IImmutableList<ToDoTask>> GetTasksAsync(this ITaskListService svc, IEnumerable<TaskList> lists, CancellationToken ct)
	{
		var asyncGetTasks = lists.Select(list => svc.GetTasksAsync(list, ct)).ToList();

		await Task.WhenAll(asyncGetTasks);

		return asyncGetTasks
			.SelectMany(getTasksTask => getTasksTask.Result)
			.OrderBy(task => task.Body?.Content ?? "")
			.ToImmutableList();
	}
}

namespace ToDo;
public interface ITaskService
{
    Task<Response<TaskData>> GetAsync(string listId, string taskId, CancellationToken ct);

    Task<Response<TaskData>> CreateAsync(string listId, TaskData newTask, CancellationToken ct);

    Task<Response<TaskData>> UpdateAsync(string listId, string taskId, TaskData updatedTask, CancellationToken ct);

    System.Threading.Tasks.Task DeleteAsync(string listId, string taskId, CancellationToken ct);
}

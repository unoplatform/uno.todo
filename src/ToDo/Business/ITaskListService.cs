namespace ToDo;

public interface ITaskListService
{
    Task<Response<List<TaskListData>>> GetAllAsync(CancellationToken ct);

    Task<Response<TaskListData>> GetAsync(string todoTaskListId, CancellationToken ct);

    Task<Response<HttpResponseMessage>> CreateAsync(TaskListRequestData todoList, CancellationToken ct);

    Task<Response<HttpResponseMessage>> UpdateAsync(string todoTaskListId, TaskListRequestData todoList, CancellationToken ct);

    Task<Response<HttpResponseMessage>> DeleteAsync(string todoTaskListId, CancellationToken ct);

    Task<Response<List<TaskData>?>> GetTasksAsync(string todoTaskListId, CancellationToken ct);
}

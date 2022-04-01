using ToDo.Data.Models;

namespace ToDo.Business.Interface;

public interface ITaskListService
{
    Task<ResponseService<List<TaskListData>>> GetAllAsync(CancellationToken ct);

    Task<ResponseService<TaskListData>> GetAsync(string todoTaskListId, CancellationToken ct);

    Task<ResponseService<HttpResponseMessage>> CreateAsync(TaskListRequestData todoList, CancellationToken ct);

    Task<ResponseService<HttpResponseMessage>> UpdateAsync(string todoTaskListId, TaskListRequestData todoList, CancellationToken ct);

    Task<ResponseService<HttpResponseMessage>> DeleteAsync(string todoTaskListId, CancellationToken ct);

    Task<ResponseService<List<TaskData>?>> GetTasksAsync(string todoTaskListId, CancellationToken ct);


}

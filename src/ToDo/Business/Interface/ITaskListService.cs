using ToDo.Data.Models;
using ToDo.Data.Models.DataModels;


namespace ToDo.Business.Interface;

public interface ITaskListService
{
    Task<ResponseService<List<TaskListData>>> GetAllAsync(CancellationToken ct);

    Task<ResponseService<TaskListData>> GetAsync(string todoTaskListId, CancellationToken ct);

    Task<HttpResponseMessage> CreateAsync(TaskListRequestData todoList, CancellationToken ct);

    Task<HttpResponseMessage> UpdateAsync(string todoTaskListId, TaskListRequestData todoList, CancellationToken ct);

    Task<HttpResponseMessage> DeleteAsync(string todoTaskListId, CancellationToken ct);

    Task<ResponseService<List<TaskData>?>> GetTasksAsync(string todoTaskListId, CancellationToken ct);


}

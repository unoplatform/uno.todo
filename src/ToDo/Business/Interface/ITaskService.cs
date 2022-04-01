

using ToDo.Data.Models;

namespace ToDo.Business.Interface
{
    public interface ITaskService
    {
        Task<ResponseService<TaskData>> GetAsync(string listId, string taskId, CancellationToken ct);

        Task<ResponseService<TaskData>> CreateAsync(string listId, TaskData newTask, CancellationToken ct);

        Task<ResponseService<TaskData>> UpdateAsync(string listId, string taskId, TaskData updatedTask, CancellationToken ct);

        Task DeleteAsync(string listId, string taskId, CancellationToken ct);
    }
}


using ToDo.Data.Models.DataModels;

namespace ToDo.Business.Interface
{
    public interface ITaskService
    {
        Task<TaskData> GetAsync(string listId, string taskId, CancellationToken ct);

        Task<TaskData> CreateAsync(string listId,object newTask, CancellationToken ct);

        Task<TaskData> UpdateAsync(string listId, string taskId, object updatedTask, CancellationToken ct);

        Task DeleteAsync(string listId, string taskId, CancellationToken ct);
    }
}

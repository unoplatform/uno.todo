
using ToDo.Models.DataModels;

namespace ToDo.Services.Interface
{
    public interface ITodoTaskService
    {
        Task<TodoTask> GetTask(string listId, string taskId, CancellationToken ct);

        Task<TodoTask> CreateTask(string listId,object newTask, CancellationToken ct);

        Task<TodoTask> UpdateTask(string listId, string taskId, object updatedTask, CancellationToken ct);

        Task DeleteTask(string listId, string taskId, CancellationToken ct);
    }
}

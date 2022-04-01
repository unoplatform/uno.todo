using Refit;
using ToDo.Data.Models.DataModels;

namespace ToDo.Services
{
    [Headers("Content-Type: application/json")]
    public interface ITaskEndpoint
    {
        [Get("/todo/lists/{listId}/tasks/{taskId}")]
        [Headers("Authorization: Bearer")]
        Task<TaskData> GetAsync(string listId, string taskId, CancellationToken ct);

        [Post("/todo/lists/{listId}/tasks")]
        [Headers("Authorization: Bearer")]
        Task<TaskData> CreateAsync(string listId, [Body] object newTask, CancellationToken ct);

        [Patch("/todo/lists/{listId}/tasks/{taskId}")]
        [Headers("Authorization: Bearer")]
        Task<TaskData> UpdateAsync(string listId, string taskId, [Body] object updatedTask, CancellationToken ct);

        [Delete("/todo/lists/{listId}/tasks/{taskId}")]
        [Headers("Authorization: Bearer")]
        Task DeleteAsync(string listId, string taskId, CancellationToken ct);
    }
}

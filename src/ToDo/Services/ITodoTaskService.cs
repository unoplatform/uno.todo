using Refit;
using ToDo.Models.DataModels;

namespace ToDo.Services
{
    public interface ITodoTaskService
    {
        [Get("/todo/lists/{listId}/tasks/{taskId}")]
        Task<TodoTask> GetTask(string listId, string taskId, [Header("Authorization")] string auth);

        [Post("/todo/lists/{listId}/tasks")]
        Task<TodoTask> CreateTask(string listId, [Body] object newTask, [Header("Authorization")] string auth);

        [Patch("/todo/lists/{listId}/tasks/{taskId}")]
        Task<TodoTask> UpdateTask(string listId, string taskId, [Body] object updatedTask, [Header("Authorization")] string auth);

        [Delete("/todo/lists/{listId}/tasks/{taskId}")]
        Task DeleteTask(string listId, string taskId, [Header("Authorization")] string auth);
    }
}

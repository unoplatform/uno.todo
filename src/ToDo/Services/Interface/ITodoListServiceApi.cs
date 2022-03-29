using Newtonsoft.Json.Linq;
using Refit;
using ToDo.Models;

namespace ToDo.Services.Interface;

[Headers("Content-Type: application/json")]
public interface ITodoListServiceApi
{
    [Get("/todo/lists")]
    [Headers("Authorization: Bearer")]
    Task<ResponseTodoListObj> GetTodoListsAsync(CancellationToken ct);

    [Get("/todo/lists/{todoTaskListId}")]
    [Headers("Authorization: Bearer")]
    Task<TodoList> GetTodoListAsync(string todoTaskListId, CancellationToken ct);

    [Post("/todo/lists")]
    [Headers("Authorization: Bearer")]
    Task<HttpResponseMessage> CreateTodoListAsync([Body] CreateUpdateTodoListObj todoList, CancellationToken ct);

    [Patch("/todo/lists/{todoTaskListId}")]
    [Headers("Authorization: Bearer")]
    Task<HttpResponseMessage> UpdateTodoListAsync(string todoTaskListId, [Body] CreateUpdateTodoListObj todoList, CancellationToken ct);

    [Delete("/todo/lists/{todoTaskListId}")]
    [Headers("Authorization: Bearer")]
    Task<HttpResponseMessage> DeleteTodoListAsync(string todoTaskListId, CancellationToken ct);
}

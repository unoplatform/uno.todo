using ToDo.Models;

namespace ToDo.Services.Interface;

public interface ITodoListService
{
    Task<ResponseService<List<TodoList>>> GetTodoListsAsync(CancellationToken ct);

    Task<ResponseService<TodoList>> GetTodoListAsync(string todoTaskListId, CancellationToken ct);

    Task<HttpResponseMessage> CreateTodoListAsync(CreateUpdateTodoListObj todoList, CancellationToken ct);

    Task<HttpResponseMessage> UpdateTodoListAsync(string todoTaskListId, CreateUpdateTodoListObj todoList, CancellationToken ct);

    Task<HttpResponseMessage> DeleteTodoListAsync(string todoTaskListId, CancellationToken ct);
}

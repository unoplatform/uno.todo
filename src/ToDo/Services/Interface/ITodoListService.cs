using ToDo.Models;

namespace ToDo.Services.Interface;

public interface ITodoListService
{
    Task<List<TodoList>> GetTodoListsAsync(CancellationToken ct);

    Task<TodoList> GetTodoListAsync(string todoTaskListId, CancellationToken ct);

    Task<bool> CreateTodoListAsync(CreateUpdateTodoListObj todoList, CancellationToken ct);

    Task<bool> UpdateTodoListAsync(string todoTaskListId, CreateUpdateTodoListObj todoList, CancellationToken ct);

    Task<bool> DeleteTodoListAsync(string todoTaskListId, CancellationToken ct);
}

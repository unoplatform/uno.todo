namespace ToDo;

[Headers("Content-Type: application/json")]
public interface IToDoTaskEndpoint
{
	[Get("/todo/lists/{listId}/tasks/{taskId}")]
	[Headers("Authorization: Bearer")]
	Task<ToDoTaskData> GetAsync(string listId, string taskId, CancellationToken ct);

	[Post("/todo/lists/{listId}/tasks")]
	[Headers("Authorization: Bearer")]
	Task<ToDoTaskData> CreateAsync(string listId, [Body] ToDoTaskData newTask, CancellationToken ct);

	[Patch("/todo/lists/{listId}/tasks/{taskId}")]
	[Headers("Authorization: Bearer")]
	Task<ToDoTaskData> UpdateAsync(string listId, string taskId, [Body] ToDoTaskData updatedTask, CancellationToken ct);

	[Delete("/todo/lists/{listId}/tasks/{taskId}")]
	[Headers("Authorization: Bearer")]
	Task DeleteAsync(string listId, string taskId, CancellationToken ct);
}

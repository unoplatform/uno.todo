using Refit;

namespace ToDo;

[Headers("Content-Type: application/json")]
public interface ITaskListEndpoint
{
	[Get("/todo/lists")]
	[Headers("Authorization: Bearer")]
	Task<TaskReponseData<ToDoTaskListData>> GetAllAsync(CancellationToken ct);

	[Get("/todo/lists/{todoTaskListId}")]
	[Headers("Authorization: Bearer")]
	Task<ToDoTaskListData> GetAsync(string todoTaskListId, CancellationToken ct);

	[Post("/todo/lists")]
	[Headers("Authorization: Bearer")]
	Task<HttpResponseMessage> CreateAsync([Body] TaskListRequestData todoList, CancellationToken ct);

	[Patch("/todo/lists/{todoTaskListId}")]
	[Headers("Authorization: Bearer")]
	Task<HttpResponseMessage> UpdateAsync(string todoTaskListId, [Body] TaskListRequestData todoList, CancellationToken ct);

	[Delete("/todo/lists/{todoTaskListId}")]
	[Headers("Authorization: Bearer")]
	Task<HttpResponseMessage> DeleteAsync(string todoTaskListId, CancellationToken ct);

	[Get("/todo/lists/{todoTaskListId}/tasks")]
	[Headers("Authorization: Bearer")]
	Task<TaskReponseData<ToDoTaskData>> GetTasksAsync(string todoTaskListId, CancellationToken ct);
}

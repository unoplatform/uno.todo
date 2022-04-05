using Refit;

namespace ToDo;

[Headers("Content-Type: application/json")]
public interface IToDoTaskListEndpoint
{
	[Get("/todo/lists")]
	[Headers("Authorization: Bearer")]
	Task<ToDoTaskReponseData<ToDoTaskListData>> GetAllAsync(CancellationToken ct);

	[Get("/todo/lists/{todoTaskListId}")]
	[Headers("Authorization: Bearer")]
	Task<ToDoTaskListData> GetAsync(string todoTaskListId, CancellationToken ct);

	[Post("/todo/lists")]
	[Headers("Authorization: Bearer")]
	Task<HttpResponseMessage> CreateAsync([Body] ToDoTaskListRequestData todoList, CancellationToken ct);

	[Patch("/todo/lists/{todoTaskListId}")]
	[Headers("Authorization: Bearer")]
	Task<HttpResponseMessage> UpdateAsync(string todoTaskListId, [Body] ToDoTaskListRequestData todoList, CancellationToken ct);

	[Delete("/todo/lists/{todoTaskListId}")]
	[Headers("Authorization: Bearer")]
	Task<HttpResponseMessage> DeleteAsync(string todoTaskListId, CancellationToken ct);

	[Get("/todo/lists/{todoTaskListId}/tasks")]
	[Headers("Authorization: Bearer")]
	Task<ToDoTaskReponseData<ToDoTaskData>> GetTasksAsync(string todoTaskListId, CancellationToken ct);
}

namespace ToDo;

[Headers("Content-Type: application/json")]
public interface ITaskEndpoint
{
	[Get("/todo/lists/{listId}/tasks/{taskId}")]
	[Headers("Authorization: Bearer")]
	Task<TaskData> GetAsync(string listId, string taskId, CancellationToken ct);

	[Post("/todo/lists/{listId}/tasks")]
	[Headers("Authorization: Bearer")]
	Task<TaskData> CreateAsync(string listId, [Body] TaskData newTask, CancellationToken ct);

	[Patch("/todo/lists/{listId}/tasks/{taskId}")]
	[Headers("Authorization: Bearer")]
	Task<TaskData> UpdateAsync(string listId, string taskId, [Body] TaskData updatedTask, CancellationToken ct);

	[Delete("/todo/lists/{listId}/tasks/{taskId}")]
	[Headers("Authorization: Bearer")]
	Task DeleteAsync(string listId, string taskId, CancellationToken ct);

	[Get("/todo/lists/{todoTaskListId}/tasks")]
	[Headers("Authorization: Bearer")]
	Task<TaskReponseData<TaskData>> GetAsync(string todoTaskListId, CancellationToken ct);

	[Get("/tasks/allTasks?filter=contains(displayName,'{displayName}')")]
	[Headers("Authorization: Bearer")]
	Task<TaskReponseData<TaskData>> GetByFilterAsync(string displayName, CancellationToken ct);

	[Get("/tasks/allTasks")]
	[Headers("Authorization: Bearer")]
	Task<TaskReponseData<TaskData>> GetAllAsync(CancellationToken ct);

	[Post("/me/tasks/lists/{baseTaskListId}/tasks/{baseTaskId}/checklistItems")]
	[Headers("Authorization: Bearer")]
	Task<CheckListItemData> AddStepAsync(string baseTaskListId, string baseTaskId, CheckListItemData checkListItem, CancellationToken ct);

	[Get("/me/tasks/lists/{baseTaskListId}/tasks/{baseTaskId}/checklistItems")]
	[Headers("Authorization: Bearer")]
	Task<TaskReponseData<CheckListItemData>> GetStepsAsync(string baseTaskListId, string baseTaskId, CancellationToken ct);

	[Delete("/me/tasks/lists/{baseTaskListId}/tasks/{baseTaskId}/checklistItems/{checklistItemId}")]
	[Headers("Authorization: Bearer")]
	Task DeleteStepAsync(string baseTaskListId, string baseTaskId, string checklistItemId, CancellationToken ct);

	[Patch("/me/tasks/lists/{baseTaskListId}/tasks/{baseTaskId}/checklistItems/{checklistItemId}")]
	[Headers("Authorization: Bearer")]
	Task<CheckListItemData> UpdateStepAsync(string baseTaskListId, string baseTaskId, string checklistItemId, CheckListItemData checkListItem, CancellationToken ct);
}

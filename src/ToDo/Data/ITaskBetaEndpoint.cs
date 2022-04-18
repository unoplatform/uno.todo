namespace ToDo;

[Headers("Content-Type: application/json")]
public interface ITaskBetaEndpoint
{
	[Get("/tasks/allTasks?filter=contains(displayName,'{displayName}')")]
	[Headers("Authorization: Bearer")]
	Task<TaskReponseData<TaskData>> GetByFilterAsync(string displayName, CancellationToken ct);

	[Get("/tasks/allTasks")]
	[Headers("Authorization: Bearer")]
	Task<TaskReponseData<TaskData>> GetAllAsync(CancellationToken ct);
}

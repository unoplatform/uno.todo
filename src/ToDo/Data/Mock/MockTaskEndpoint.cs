

namespace ToDo.Data.Mock;

public class MockTaskEndpoint : ITaskEndpoint
{

	private readonly MockTaskListEndpoint _listEndpoint;

	public MockTaskEndpoint(
		ITaskListEndpoint listEndpoint)
	{
		_listEndpoint = (listEndpoint as MockTaskListEndpoint)!;
	}

	public async Task<TaskData> CreateAsync(string listId, [Body] CreateTaskData newTask, CancellationToken ct)
	{
		var createdTask = new TaskData()
		{
			Id = Guid.NewGuid().ToString("N"),
			Importance = newTask.Importance,
			CreatedDateTime = DateTime.Now,
			Body = newTask.Body,
			Title = newTask.Title,
			DueDateTime = newTask.DueDateTime,
			IsReminderOn = newTask.IsReminderOn,
			Status = newTask.Status
		};
		await _listEndpoint.AddTaskToList(listId, createdTask);
		return createdTask;
	}
	public async Task DeleteAsync(string listId, string taskId, CancellationToken ct)
	{
		await _listEndpoint.DeleteTaskFromList(listId, taskId);
	}

	public async Task<TaskReponseData<TaskData>> GetAsync(string todoTaskListId, CancellationToken ct)
	{
		var tasks = await _listEndpoint.LoadListTasks(todoTaskListId);

		return new TaskReponseData<TaskData> { Value = tasks.ToList() };
	}

	public async Task<TaskReponseData<TaskData>> GetAllAsync(CancellationToken ct) => await _listEndpoint.GetAllTasksAsync( ct: ct);

	public async Task<TaskData> GetAsync(string listId, string taskId, CancellationToken ct)
	{
		var tasks = await GetAsync(listId, ct);

		return tasks.Value.First(x => x.Id == taskId);
	}

	public async Task<TaskReponseData<TaskData>> GetByFilterAsync(string displayName, CancellationToken ct) => await _listEndpoint.GetAllTasksAsync(displayName, ct);

	public async Task<TaskData> UpdateAsync(string listId, string taskId, [Body] TaskData updatedTask, CancellationToken ct)
	{
		await _listEndpoint.UpdateTaskInList(listId, updatedTask);
		return updatedTask;
	}
}

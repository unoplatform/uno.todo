﻿

namespace ToDo.Data.Mock;

public class MockTaskEndpoint : ITaskEndpoint
{

	private readonly MockTaskListEndpoint _listEndpoint;
	public MockTaskEndpoint(
		ITaskListEndpoint listEndpoint)
	{
		_listEndpoint = (listEndpoint as MockTaskListEndpoint)!;
	}

	public async Task<TaskData> CreateAsync(string listId, [Body] TaskData newTask, CancellationToken ct)
	{
		await _listEndpoint.AddTaskToList(listId, newTask);
		return newTask;
	}
	public async Task DeleteAsync(string listId, string taskId, CancellationToken ct)
	{
		await _listEndpoint.DeleteTaskFromList(listId, taskId);
	}
	public async Task<TaskData> GetAsync(string listId, string taskId, CancellationToken ct)
	{
		var tasks = await _listEndpoint.GetTasksAsync(listId, ct);

		return tasks.Value.First(x => x.Id == taskId);
	}
	public async Task<TaskData> UpdateAsync(string listId, string taskId, [Body] TaskData updatedTask, CancellationToken ct)
	{
		await _listEndpoint.UpdateTaskInList(listId, updatedTask);
		return updatedTask;
	}
}

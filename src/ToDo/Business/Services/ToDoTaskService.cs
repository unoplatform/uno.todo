using System;
using System.Linq;

namespace ToDo.Business;

public class ToDoTaskService : IToDoTaskService
{
	private readonly IToDoTaskEndpoint _client;

	public ToDoTaskService(IToDoTaskEndpoint client)
	{
		_client = client;
	}

	/// <inheritdoc />
	public async Task<ToDoTask> GetAsync(string listId, string taskId, CancellationToken ct)
		=> new (listId, await _client.GetAsync(listId, taskId, ct) ?? throw new InvalidOperationException($"Cannot get task with id {taskId} (list: {listId})"));

	/// <inheritdoc />
	public async Task<ToDoTask> CreateAsync(ToDoTaskList list, ToDoTask newTask, CancellationToken ct)
		=> new(list, await _client.CreateAsync(list.Id, newTask.ToData(), ct));

	/// <inheritdoc />
	public async Task<ToDoTask> UpdateAsync(ToDoTask updatedTask, CancellationToken ct)
		=> new(updatedTask.ListId, await _client.UpdateAsync(updatedTask.ListId, updatedTask.Id, updatedTask.ToData(), ct));

	/// <inheritdoc />
	public async Task DeleteAsync(ToDoTask task, CancellationToken ct)
		=> await _client.DeleteAsync(task.ListId, task.Id, ct);
}

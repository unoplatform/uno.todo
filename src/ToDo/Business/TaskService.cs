﻿namespace ToDo.Business;

public class TaskService : ITaskService
{
	private readonly ITaskEndpoint _client;
	private readonly IMessenger _messenger;

	public TaskService(ITaskEndpoint client, IMessenger messenger)
	{
		_client = client;
		_messenger = messenger;
	}

	/// <inheritdoc />
	public async Task<ToDoTask> GetAsync(string listId, string taskId, CancellationToken ct)
		=> new (listId, await _client.GetAsync(listId, taskId, ct) ?? throw new InvalidOperationException($"Cannot get task with id {taskId} (list: {listId})"));

	/// <inheritdoc />
	public async Task CreateAsync(TaskList list, ToDoTask newTask, CancellationToken ct)
	{
		var createdTask = await _client.CreateAsync(list.Id, newTask.ToData(), ct);

		_messenger.Send(new EntityMessage<ToDoTask>(EntityChange.Create, new (list, createdTask)));
	}

	/// <inheritdoc />
	public async Task UpdateAsync(ToDoTask task, CancellationToken ct)
	{
		var updatedTask = await _client.UpdateAsync(task.ListId, task.Id, task.ToData(), ct);

		_messenger.Send(new EntityMessage<ToDoTask>(EntityChange.Update, new (task.ListId, updatedTask)));
	}

	/// <inheritdoc />
	public async Task DeleteAsync(ToDoTask task, CancellationToken ct)
	{
		await _client.DeleteAsync(task.ListId, task.Id, ct);

		_messenger.Send(new EntityMessage<ToDoTask>(EntityChange.Delete, task));
	}

	/// <inheritdoc />
	public async ValueTask<IEnumerable<ToDoTask>> GetAllAsync(string displayName = "", CancellationToken ct = default)
	{
		var todoTasksList = (await _client.GetAllAsync(ct)).Value;

		if (String.IsNullOrWhiteSpace(displayName))
			todoTasksList = (await _client.GetAllAsync(ct)).Value;

		return (todoTasksList ?? Enumerable.Empty<TaskData>())
				.Select(data => {
					var taskListId = data.ParentList?.Id ??
						throw new InvalidOperationException("The API did not provide list information.");
					return new ToDoTask(taskListId, data);
				})
				.ToList();
	}
}

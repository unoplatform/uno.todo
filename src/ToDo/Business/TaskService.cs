namespace ToDo.Business;

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
		=> new(listId, await _client.GetAsync(listId, taskId, ct) ?? throw new InvalidOperationException($"Cannot get task with id {taskId} (list: {listId})"));

	/// <inheritdoc />
	public async Task CreateAsync(TaskList list, ToDoTask newTask, CancellationToken ct)
	{
		var createdTask = await _client.CreateAsync(list.Id, newTask.ToData(), ct);

		_messenger.Send(new EntityMessage<ToDoTask>(EntityChange.Create, new(list, createdTask)), list.Id);
	}

	/// <inheritdoc />
	public async Task UpdateAsync(ToDoTask task, CancellationToken ct)
	{
		var updatedTask = await _client.UpdateAsync(task.ListId, task.Id, task.ToData(), ct);

		// Send updates to listeners of both the list and the individual task (in case the task page is open)
		_messenger.Send(new EntityMessage<ToDoTask>(EntityChange.Update, new(task.ListId, updatedTask)), task.ListId);
		_messenger.Send(new EntityMessage<ToDoTask>(EntityChange.Update, new(task.ListId, updatedTask)), task.Id);
	}

	/// <inheritdoc />
	public async Task DeleteAsync(ToDoTask task, CancellationToken ct)
	{
		await _client.DeleteAsync(task.ListId, task.Id, ct);

		_messenger.Send(new EntityMessage<ToDoTask>(EntityChange.Delete, task), task.ListId);
	}

	/// <inheritdoc />
	public async ValueTask<IImmutableList<ToDoTask>> GetAllAsync(string displayName = "", CancellationToken ct = default)
	{
		Task<TaskReponseData<TaskData>> getMethod = string.IsNullOrWhiteSpace(displayName) ? _client.GetAllAsync(ct) : _client.GetByFilterAsync(displayName, ct);
		return ((await getMethod).Value ?? Enumerable.Empty<TaskData>())
		.Where(data => data.ParentList?.Id is not null)
		.Select(data =>
		{
			return new ToDoTask(data.ParentList?.Id ?? string.Empty, data);
		}).ToImmutableList();
	}
}

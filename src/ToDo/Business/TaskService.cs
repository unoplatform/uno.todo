namespace ToDo.Business;

public class TaskService : ITaskService
{
	private readonly ITaskEndpoint _client;
	private readonly ITaskBetaEndpoint _betaClient;
	private readonly IMessenger _messenger;

	public TaskService(ITaskEndpoint client,
		ITaskBetaEndpoint betaClient, IMessenger messenger)
	{
		_client = client;
		_betaClient = betaClient;
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

	public async ValueTask<IImmutableList<ToDoTask>> GetByFilterAsync(string displayName, CancellationToken ct)
		=> ((await _betaClient.GetByFilterAsync(displayName, ct)).Value ?? Enumerable.Empty<TaskData>())
			.Select(data => new ToDoTask(data))
			.ToImmutableList();
	public async ValueTask<IImmutableList<ToDoTask>> GetAllAsync(CancellationToken ct)
		=> ((await _betaClient.GetAllAsync(ct)).Value ?? Enumerable.Empty<TaskData>())
			.Select(data => new ToDoTask(data))
			.ToImmutableList();

}

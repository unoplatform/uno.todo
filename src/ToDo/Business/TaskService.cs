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

		_messenger.Send(new EntityMessage<ToDoTask>(EntityChange.Create, new(list, createdTask)));
	}

	/// <inheritdoc />
	public async Task UpdateAsync(ToDoTask task, CancellationToken ct)
	{
		var updatedTask = await _client.UpdateAsync(task.ListId, task.Id, task.ToData(), ct);

		// Send updates to listeners of both the list and the individual task (in case the task page is open)
		_messenger.Send(new EntityMessage<ToDoTask>(EntityChange.Update, new(task.ListId, updatedTask)));
	}

	/// <inheritdoc />
	public async Task DeleteAsync(ToDoTask task, CancellationToken ct)
	{
		await _client.DeleteAsync(task.ListId, task.Id, ct);

		_messenger.Send(new EntityMessage<ToDoTask>(EntityChange.Delete, task));
	}

	/// <inheritdoc />
	public async ValueTask<IImmutableList<ToDoTask>> GetAsync(TaskList list, CancellationToken ct)
	{
		if (list.WellknownListName == TaskList.WellknownListNames.Important)
		{
			return (await GetAllAsync(ct: ct))
				.Where(task => task.IsImportant)
				.ToImmutableList();
		}
		else
		{
			return ((await _client.GetAsync(list.Id, ct)).Value ?? Enumerable.Empty<TaskData>())
				.Select(data => new ToDoTask(list.Id, data))
				.ToImmutableList();
		}
	}

	/// <inheritdoc />
	public async ValueTask<IImmutableList<ToDoTask>> GetAllAsync(string displayName = "", CancellationToken ct = default)
	{
		var response = await (displayName is { Length: > 0 }
			? _client.GetByFilterAsync(displayName, ct)
			: _client.GetAllAsync(ct));

		return (response.Value ?? Enumerable.Empty<TaskData>())
			.Where(data => data.ParentList?.Id is not null)
			.Select(data => new ToDoTask(data.ParentList!.Id!, data))
			.ToImmutableList();
	}

	/// <inheritdoc />

	public async ValueTask AddStepAsync(string baseTaskListId, string baseTaskId,
		CheckListItem checkListItem, CancellationToken ct)
	{
		var createdStep = await _client.AddStepAsync(baseTaskListId, baseTaskId, checkListItem.ToData(), ct);

		//TODO: add messenger notification
		//_messenger.Send(new EntityMessage<CheckListItem>(EntityChange.Create, new(baseTaskId, createdStep)), baseTaskId);
	}

	/// <inheritdoc />

	public async ValueTask<IImmutableList<CheckListItem>> GetStepsAsync(string baseTaskListId,
		string baseTaskId, CancellationToken ct)
		=> ((await _client.GetStepsAsync(baseTaskListId, baseTaskId, ct)).Value ?? Enumerable.Empty<CheckListItemData>())
				.Select(data => new CheckListItem(data))
				.ToImmutableList();

	/// <inheritdoc />
	public async ValueTask DeleteStepAsync(string baseTaskListId, string baseTaskId, string checklistItemId,
		CancellationToken ct)
	{
		await _client.DeleteStepAsync(baseTaskListId, baseTaskId, checklistItemId, ct);

		//TODO: suscribe notifications for deleting task step
		//_messenger.Send(new EntityMessage<CheckListItem>(EntityChange.Delete, checklistItemId), baseTaskId);
	}

	/// <inheritdoc />
	public async ValueTask UpdateStepAsync(string baseTaskListId, string baseTaskId,
		 CheckListItem checkListItem, CancellationToken ct)
	{
		var updatedTaskStep = await _client.UpdateStepAsync(baseTaskListId, baseTaskId,
			checkListItem.Id, checkListItem.ToData(), ct);

		//TODO: suscribe update notification
		//_messenger.Send(new EntityMessage<CheckListItem>(EntityChange.Update, new(baseTaskId, updatedTaskStep)), baseTaskId);
	}
}

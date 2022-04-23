
namespace ToDo.Business;

public class TaskListService : ITaskListService
{
	private readonly ITaskListEndpoint _client;
	private readonly ITaskService _taskSvc;
	private readonly IMessenger _messenger;

	public TaskListService(ITaskListEndpoint client, ITaskService taskSvc, IMessenger messenger)
	{
		_client = client;
		_taskSvc = taskSvc;
		_messenger = messenger;
	}

	/// <inheritdoc />
	public async ValueTask<IImmutableList<TaskList>> GetAllAsync(CancellationToken ct)
		=> ((await _client.GetAllAsync(ct)).Value ?? Enumerable.Empty<TaskListData>())
			.Select(data => new TaskList(data))
			.ToImmutableList();

	/// <inheritdoc />
	public async Task<TaskList> GetAsync(string listId, CancellationToken ct)
		=> new(await _client.GetAsync(listId, ct));

	/// <inheritdoc />
	public async Task CreateAsync(string displayName, CancellationToken ct)
	{
		var createdList = await _client.CreateAsync(new TaskListRequestData { DisplayName = displayName }, ct);

		_messenger.Send(new EntityMessage<TaskList>(EntityChange.Create, new (createdList)));
	}

	/// <inheritdoc />
	public async Task UpdateAsync(TaskList list, CancellationToken ct)
	{
		var updatedList = await _client.UpdateAsync(list.Id, new TaskListRequestData { DisplayName = list.DisplayName }, ct);

		// Send message with both list.Id (in case TaskList page is open) and without list.Id (for updating HomePage)
		_messenger.Send(new EntityMessage<TaskList>(EntityChange.Update, new(updatedList)));
		_messenger.Send(new EntityMessage<TaskList>(EntityChange.Update, new(updatedList)), list.Id);
	}

	/// <inheritdoc />
	public async Task DeleteAsync(TaskList list, CancellationToken ct)
	{
		(await _client.DeleteAsync(list.Id, ct)).EnsureSuccessStatusCode();

		_messenger.Send(new EntityMessage<TaskList>(EntityChange.Delete, list));
	}

	/// <inheritdoc />
	public async ValueTask<IImmutableList<ToDoTask>> GetTasksAsync(TaskList list, CancellationToken ct)
	{
		if (list == TaskList.Important)
		{
			return (await _taskSvc.GetAllAsync(ct: ct))
				.Where(task => task.IsImportant)
				.ToImmutableList();
		}
		else
		{
			return ((await _client.GetTasksAsync(list.Id, ct)).Value ?? Enumerable.Empty<TaskData>())
				.Select(data => new ToDoTask(list.Id, data))
				.ToImmutableList();
		}
	}
}

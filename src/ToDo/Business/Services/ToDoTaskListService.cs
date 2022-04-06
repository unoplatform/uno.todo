using System;
using System.Collections.Immutable;
using System.Linq;

namespace ToDo.Business;

public class ToDoTaskListService : IToDoTaskListService
{
	private readonly IToDoTaskListEndpoint _client;
	private readonly IMessenger _messenger;

	public ToDoTaskListService(IToDoTaskListEndpoint client, IMessenger messenger)
	{
		_client = client;
		_messenger = messenger;
	}

	/// <inheritdoc />
	public async ValueTask<IImmutableList<ToDoTaskList>> GetAllAsync(CancellationToken ct)
		=> ((await _client.GetAllAsync(ct)).Value ?? Enumerable.Empty<ToDoTaskListData>())
			.Select(data => new ToDoTaskList(data))
			.ToImmutableList();

	/// <inheritdoc />
	public async Task<ToDoTaskList> GetAsync(string listId, CancellationToken ct)
		=> new(await _client.GetAsync(listId, ct));

	/// <inheritdoc />
	public async Task CreateAsync(string displayName, CancellationToken ct)
	{
		var createdList = await _client.CreateAsync(new ToDoTaskListRequestData { DisplayName = displayName }, ct);

		_messenger.Notify(new EntityMessage<ToDoTaskList>(EntityChange.Create, new (createdList)));
	}

	/// <inheritdoc />
	public async Task UpdateAsync(ToDoTaskList list, CancellationToken ct)
	{
		var updatedList = await _client.UpdateAsync(list.Id, new ToDoTaskListRequestData { DisplayName = list.DisplayName }, ct);

		_messenger.Notify(new EntityMessage<ToDoTaskList>(EntityChange.Update, new(updatedList)));
	}

	/// <inheritdoc />
	public async Task DeleteAsync(ToDoTaskList list, CancellationToken ct)
	{
		(await _client.DeleteAsync(list.Id, ct)).EnsureSuccessStatusCode();

		_messenger.Notify(new EntityMessage<ToDoTaskList>(EntityChange.Delete, list));
	}

	/// <inheritdoc />
	public async Task<IImmutableList<ToDoTask>> GetTasksAsync(string listId, CancellationToken ct)
		=> ((await _client.GetTasksAsync(listId, ct)).Value ?? Enumerable.Empty<ToDoTaskData>())
			.Select(data => new ToDoTask(listId, data))
			.ToImmutableList();
}

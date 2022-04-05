using System;
using System.Collections.Immutable;
using System.Linq;

namespace ToDo.Business;

public class ToDoTaskListService : IToDoTaskListService
{
	private readonly ITaskListEndpoint _client;

	public ToDoTaskListService(ITaskListEndpoint client)
	{
		_client = client;
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
		=> (await _client.CreateAsync(new TaskListRequestData { DisplayName = displayName }, ct)).EnsureSuccessStatusCode();

	/// <inheritdoc />
	public async Task UpdateAsync(ToDoTaskList list, CancellationToken ct)
		=> (await _client.UpdateAsync(list.Id, new TaskListRequestData { DisplayName = list.DisplayName }, ct)).EnsureSuccessStatusCode();

	/// <inheritdoc />
	public async Task DeleteAsync(ToDoTaskList list, CancellationToken ct)
		=> (await _client.DeleteAsync(list.Id, ct)).EnsureSuccessStatusCode();

	/// <inheritdoc />
	public async Task<IImmutableList<ToDoTask>> GetTasksAsync(string listId, CancellationToken ct)
		=> ((await _client.GetTasksAsync(listId, ct)).Value ?? Enumerable.Empty<ToDoTaskData>())
			.Select(data => new ToDoTask(listId, data))
			.ToImmutableList();
}



namespace ToDo.Data.Mock;

public class MockTaskListEndpoint : ITaskListEndpoint
{
	private const string ListDataFile = "lists.json";
	private const string TasksDataFile = "tasks.json";

	private readonly IJsonDataService<TaskListData> _listsDataService;
	private readonly IJsonDataService<TaskData> _tasksDataService;
	public MockTaskListEndpoint(
		IJsonDataService<TaskListData> listsDataService,
		IJsonDataService<TaskData> tasksDataService)
	{
		_listsDataService = listsDataService;
		_listsDataService.DataFile = ListDataFile;
		_tasksDataService = tasksDataService;
		_tasksDataService.DataFile = TasksDataFile;
	}

	private IList<TaskListData>? data;
	private IDictionary<string, IList<TaskData>> taskData = new Dictionary<string, IList<TaskData>>();

	private async Task Load()
	{
		if (data is null)
		{
			data = (await _listsDataService.GetEntities()).ToList();
		}
	}

	private async Task<IList<TaskData>> LoadListTasks(string listId)
	{
		if (taskData.TryGetValue(listId, out var existingTasks))
		{
			return existingTasks;
		}

		var tasks = (await _tasksDataService.GetEntities()).ToList();
		taskData[listId] = tasks;
		return tasks;
	}

	public async Task<TaskListData> CreateAsync(TaskListRequestData todoList, CancellationToken ct)
	{
		await Load();

		var list = new TaskListData { Id = Guid.NewGuid().ToString(), DisplayName = todoList.DisplayName };
		data?.Add(list);
		return list;
	}

	public async Task<HttpResponseMessage> DeleteAsync(string todoTaskListId, CancellationToken ct)
	{
		await Load();
		var list = data.FirstOrDefault(x => x.Id == todoTaskListId);
		if (list is not null)
		{
			data?.Remove(list);
		}
		return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
	}

	public async Task<TaskReponseData<TaskListData>> GetAllAsync(CancellationToken ct)
	{
		await Load();
		return new TaskReponseData<TaskListData> { Value = data.ToList() };
	}

	public async Task<TaskListData> GetAsync(string todoTaskListId, CancellationToken ct)
	{
		await Load();
		var list = data.FirstOrDefault(x => x.Id == todoTaskListId);
		if (list is not null)
		{
			return list;
		}

		return new TaskListData();
	}

	public async Task<TaskReponseData<TaskData>> GetTasksAsync(string todoTaskListId, CancellationToken ct)
	{
		var tasks = await LoadListTasks(todoTaskListId);

		return new TaskReponseData<TaskData> { Value = tasks.ToList() };
	}

	public async Task<TaskListData> UpdateAsync(string todoTaskListId, [Body] TaskListRequestData todoList, CancellationToken ct)
	{
		await Load();

		var list = data.FirstOrDefault(x => x.Id == todoTaskListId);
		if (list is not null)
		{
			list.DisplayName = todoList.DisplayName;
			return list;
		}

		return new TaskListData();
	}


	internal async Task AddTaskToList(string todoTaskListId, TaskData task)
	{
		var list = await LoadListTasks(todoTaskListId);
		list.Add(task);
	}

	internal async Task DeleteTaskFromList(string todoTaskListId, string taskId)
	{
		var list = await LoadListTasks(todoTaskListId);
		var task = list.FirstOrDefault(t => t.Id == taskId);
		if (task is not null)
		{
			list.Remove(task);
		}
	}

	internal async Task UpdateTaskInList(string todoTaskListId, TaskData task)
	{
		await DeleteTaskFromList(todoTaskListId, task.Id!);
		await AddTaskToList(todoTaskListId, task);
	}
}

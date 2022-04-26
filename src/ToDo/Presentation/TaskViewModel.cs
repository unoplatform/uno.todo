namespace ToDo.Presentation;

public partial class TaskViewModel : IRecipient<EntityMessage<ToDoTask>>
{
	private readonly INavigator _navigator;
	private readonly ITaskService _svc;
	private readonly IInput<ToDoTask> _entity;
	private readonly ILogger _logger;

	private TaskViewModel(
		ILogger<TaskListViewModel> logger,
		INavigator navigator,
		ITaskService svc,
		IMessenger messenger,
		IInput<ToDoTask> entity,
		ICommandBuilder delete,
		ICommandBuilder save,
		ICommandBuilder complete,
		ICommandBuilder markAsImportant,
		ICommandBuilder addTaskNote)
	{
		_logger = logger;
		_navigator = navigator;
		_svc = svc;
		_entity = entity;

		delete.Given(entity).Then(Delete);
		save.Given(entity).Then(Save);
		complete.Given(entity).Then(Complete);
		markAsImportant.Given(entity).Then(MarkAsImportant);
		addTaskNote.Execute(AddTaskNote);

		// TODO: Update this to register with token = task.Id
		messenger.Register(this);
	}


	private async ValueTask Delete(ToDoTask task, CancellationToken ct)
	{
		var response = await _navigator!.NavigateRouteForResultAsync<DialogAction>(this, "Confirm", qualifier: Qualifiers.Dialog);
		if (response is null)
		{
			return;
		}

		var result = await response.Result;
		if (result.SomeOrDefault()?.Id?.ToString() == "Y")
		{
			await _svc.DeleteAsync(task, ct);
			await _navigator.NavigateBackAsync(this, cancellation: ct);
		}
	}

	private async ValueTask AddTaskNote(CancellationToken ct)
	{
		var response = await _navigator.NavigateViewModelForResultAsync<TaskNoteViewModel,TaskBodyData>(this, cancellation: ct);
		if (response is null)
		{
			return;
		}

		//var result = await response.Result;

		//var listName = result.SomeOrDefault()?.DisplayName;
		//if (listName is not null)
		//{
		//	await _listSvc.CreateAsync(listName, ct);
		//}
	}

	private async ValueTask Complete(ToDoTask task, CancellationToken ct)
	{
		if (task.Status is null)
		{
			return;
		}
		if (task.Status.Equals("notStarted"))
		{
			task.Status = "completed";
		}
		else
		{
			task.Status = "notStarted";
		}
		await _svc.UpdateAsync(task, ct);
	}

	private async ValueTask MarkAsImportant(ToDoTask task, CancellationToken ct)
	{
		if (!task.IsImportant)
		{
			task.Importance = "high";
		}
		else
		{
			task.Importance = "normal";
		}
		await _svc.UpdateAsync(task, ct);
	}

	private async ValueTask Save(ToDoTask task, CancellationToken ct)
		=> await _svc.UpdateAsync(task, ct);

	public async void Receive(EntityMessage<ToDoTask> msg)
	{
		var ct = CancellationToken.None;
		try
		{
			if (msg.Change is EntityChange.Update)
			{
				await _entity.UpdateValue(current => current.IsSome(out var task) && task.Id == msg.Value.Id ? msg.Value : current, ct);
			}
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to apply update message.");
		}
	}
}

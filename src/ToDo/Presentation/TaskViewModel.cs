namespace ToDo.Presentation;

public partial class TaskViewModel
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
		ICommandBuilder toggleIsComplete,
		ICommandBuilder toggleIsImportant,
		ICommandBuilder addTaskNote)
	{
		_logger = logger;
		_navigator = navigator;
		_svc = svc;
		_entity = entity;

		delete.Given(entity).Then(Delete);
		save.Given(entity).Then(Save);
		toggleIsComplete.Given(entity).Then(ToggleCompleted);
		toggleIsImportant.Given(entity).Then(ToggleIsImportant);
		addTaskNote.Given(entity).Then(AddTaskNote);

		entity.Observe(messenger, task => task.Id);
	}

	private async ValueTask Delete(ToDoTask task, CancellationToken ct)
	{
		var response = await _navigator.NavigateRouteForResultAsync<DialogAction>(this, "Confirm", qualifier: Qualifiers.Dialog, cancellation: ct);
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

	private async ValueTask AddTaskNote(ToDoTask task, CancellationToken ct)
	{
		var response = await _navigator.NavigateViewModelForResultAsync<TaskNoteViewModel,TaskBodyData>(this, cancellation: ct);
		if (response is null)
		{
			return;
		}

		var result = await response.Result;

		var note = result.SomeOrDefault()?.Content;
		if (task.Body is not null)
		{
			var updatedTask = task with { Body = new TaskBodyData { Content = note, ContentType = task.Body.ContentType } };
			await _svc.UpdateAsync(updatedTask, ct);
		}
	}

	private async ValueTask Complete(ToDoTask task, CancellationToken ct)
	{
		if (task.Status is null)
		{
			return;
		}

		var updatedTask = task with { Status =  task.Status.Equals("completed")? "notStarted":"completed" };
		await _svc.UpdateAsync(updatedTask, ct);
	}

	private async ValueTask MarkAsImportant(ToDoTask task, CancellationToken ct)
	{
		if (task.Importance is null)
		{
			return;
		}
		var updatedTask = task with { Importance = task.Importance.Equals("normal") ? "high" : "normal" };

		await _svc.UpdateAsync(updatedTask, ct);
	}

	private async ValueTask AddTaskNote(ToDoTask task, CancellationToken ct)
	{
		var response = await _navigator.NavigateViewModelForResultAsync<TaskNoteViewModel, TaskBodyData>(this, cancellation: ct);
		if (response is null)
		{
			return;
		}

		var result = await response.Result;

		var note = result.SomeOrDefault()?.Content;
		if (note is not null)
		{
			// TODO: Switch to this code when compilation issue is fixed with source generation
			//var updatedNote = task.Body is not null ? task.Body with { Content = note } : new TaskBodyData { Content = note };
			var updatedNote = task.Body ?? new TaskBodyData();
			updatedNote.Content = note;

			var updatedTask = task with { Body = updatedNote };
			await _svc.UpdateAsync(updatedTask, ct);
		}
	}

	private async ValueTask ToggleCompleted(ToDoTask task, CancellationToken ct)
	{
		if (task.Status is null)
		{
			return;
		}

		var updatedTask = task with { Status = task.IsCompleted ? ToDoTask.TaskStatus.NotStarted : ToDoTask.TaskStatus.Completed };
		await _svc.UpdateAsync(updatedTask, ct);
	}

	private async ValueTask ToggleIsImportant(ToDoTask task, CancellationToken ct)
	{
		if (task.Importance is null)
		{
			return;
		}
		var updatedTask = task with { Importance = task.IsImportant ? ToDoTask.TaskImportance.Normal : ToDoTask.TaskImportance.Important };

		await _svc.UpdateAsync(updatedTask, ct);
	}

	private async ValueTask Save(ToDoTask task, CancellationToken ct)
		=> await _svc.UpdateAsync(task, ct);
}

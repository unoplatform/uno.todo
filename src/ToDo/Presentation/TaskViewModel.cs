﻿namespace ToDo.Presentation;

[ReactiveBindable]
public partial class TaskViewModel
{
	private readonly INavigator _navigator;
	private readonly ITaskService _svc;

	private TaskViewModel(
		INavigator navigator,
		ITaskService svc,
		IMessenger messenger,
		ToDoTask entity)
	{
		_navigator = navigator;
		_svc = svc;

		Entity = State<ToDoTask>.Async(this, async _ => entity);
		Entity.Observe(messenger, task => task.Id);
	}

	public IState<ToDoTask> Entity { get; }

	public ICommand Save => Command.Create(b => b.Given(Entity).Then(DoSave));
	private async ValueTask DoSave(ToDoTask task, CancellationToken ct)
		=> await _svc.UpdateAsync(task, ct);

	public ICommand Delete => Command.Create(b => b.Given(Entity).Then(DoDelete));
	private async ValueTask DoDelete(ToDoTask task, CancellationToken ct)
	{
		var response = await _navigator.NavigateRouteForResultAsync<DialogAction>(this, "ConfirmDeleteTask", qualifier: Qualifiers.Dialog, cancellation: ct);
		if (response is null)
		{
			return;
		}

		var result = await response.Result;
		if (result.SomeOrDefault()?.Id == DialogResults.Affirmative)
		{
			await _svc.DeleteAsync(task, ct);
			await _navigator.NavigateBackAsync(this, cancellation: ct);
		}
	}


	public ICommand AddDueDate => Command.Create(c => c.Given(Entity).Then(DoAddDueDate));
	private async ValueTask DoAddDueDate(ToDoTask task, CancellationToken ct)
	{
		var response = await _navigator.NavigateViewModelForResultAsync<ExpirationDateViewModel, DateTimeData>(this, data: task, cancellation: ct);
		if (response is null)
		{
			return;
		}

		var result = await response.Result;

		var date = result.SomeOrDefault()?.DateTime;
		if (date is not null)
		{
			var updated = task with { DueDateTime = (task.DueDateTime ?? new()) with { DateTime = (DateTime)date, TimeZone="UTC" } };

			await _svc.UpdateAsync(updated, ct);
		}
	}

	public ICommand AddTaskNote => Command.Create(b => b.Given(Entity).Then(DoAddTaskNote));
	private async ValueTask DoAddTaskNote(ToDoTask task, CancellationToken ct)
	{
		var response = await _navigator.NavigateViewModelForResultAsync<TaskNoteViewModel, TaskBodyData>(this, data: task, cancellation: ct);
		if (response is null)
		{
			return;
		}

		var result = await response.Result;

		var note = result.SomeOrDefault()?.Content;
		if (note is not null)
		{
			var updated = task with { Body = (task.Body ?? new()) with { Content = note } };

			await _svc.UpdateAsync(updated, ct);
		}
	}

	public ICommand ToggleIsCompleted => Command.Create(b => b.Given(Entity).Then(DoToggleIsCompleted));
	private async ValueTask DoToggleIsCompleted(ToDoTask task, CancellationToken ct)
	{
		if (task.Status is null)
		{
			return;
		}

		var updatedTask = task.ToggleIsCompleted();
		await _svc.UpdateAsync(updatedTask, ct);
	}
	
	public ICommand ToggleIsImportant => Command.Create(b => b.Given(Entity).Then(DoToggleIsImportant));
	private async ValueTask DoToggleIsImportant(ToDoTask task, CancellationToken ct)
	{
		if (task.Importance is null)
		{
			return;
		}
		var updatedTask = task.ToggleImportance();

		await _svc.UpdateAsync(updatedTask, ct);
	}
}

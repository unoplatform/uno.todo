﻿namespace ToDo.Business.Models;

public static class ToDoTaskExtensions
{
	public static ToDoTask WithToggledIsCompleted(this ToDoTask task)
	{
		return task with { Status = task.IsCompleted ? ToDoTask.TaskStatus.NotStarted : ToDoTask.TaskStatus.Completed };
	}

	public static ToDoTask WithToggledIsImportant(this ToDoTask task)
	{
		return task with { Importance = task.IsImportant ? ToDoTask.TaskImportance.Normal : ToDoTask.TaskImportance.Important };
	}

	public static ToDoTask WithDeleteDueDate(this ToDoTask task)
	{
		return task with { DueDateTime = null };
	}
}

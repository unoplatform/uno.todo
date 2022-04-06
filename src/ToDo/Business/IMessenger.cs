using System;
using System.Collections.Generic;
using System.Text;
using Uno.Extensions.Reactive;

namespace ToDo.Business;

public interface IMessenger
{
	public event EventHandler<EntityMessage<ToDoTask>> TaskChanged;

	public event EventHandler<EntityMessage<ToDoTaskList>> TaskListChanged;

	public void Notify(EntityMessage<ToDoTask> taskMessage);

	public void Notify(EntityMessage<ToDoTaskList> listMessage);
}

internal class Messenger : IMessenger
{
	/// <inheritdoc />
	public event EventHandler<EntityMessage<ToDoTask>>? TaskChanged;

	/// <inheritdoc />
	public event EventHandler<EntityMessage<ToDoTaskList>>? TaskListChanged;

	/// <inheritdoc />
	public void Notify(EntityMessage<ToDoTask> taskMessage)
		=> TaskChanged?.Invoke(this, taskMessage);

	/// <inheritdoc />
	public void Notify(EntityMessage<ToDoTaskList> listMessage)
		=> TaskListChanged?.Invoke(this, listMessage);
}

public record EntityMessage<T>(EntityChange Change, T Value)
{
}

public enum EntityChange
{
	Create,
	Update,
	Delete,
}

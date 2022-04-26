﻿namespace ToDo.Business;

public record TaskList
{
	public static TaskList Important { get; } = new("important", "Important");

	public static TaskList Tasks { get; } = new("tasks", "Tasks");

	internal TaskList(string wellknownListName, string displayName)
	{
		WellknownListName = wellknownListName;
		DisplayName = displayName;
		Id = wellknownListName;
	}

	internal TaskList(TaskListData data)
	{
		Id = data.Id ?? throw new ArgumentNullException("data.Id", "List must have a valid ID.");
		Odata = data.Odata;
		DisplayName = data.DisplayName;
		IsOwner = data.IsOwner;
		IsShared = data.IsShared;
		WellknownListName = data.WellknownListName;
	}

	public string Id { get; } // No public init: this can be set only from a data

	public string? Odata { get; init; }

	public string? DisplayName { get; init; }

	public bool IsOwner { get; init; }

	public bool IsShared { get; init; }

	public string? WellknownListName { get; init; }
}

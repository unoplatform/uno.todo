namespace ToDo.Business;

public record ToDoTask
{
	public ToDoTask()
	{
		Id = default!;
		ListId = default!;
	}

	// This ctor should be used only by business and should remain internal
	// To update a ToDoTask, use the "with" operator.
	internal ToDoTask(TaskList list, TaskData data)
		: this(list.Id, data)
	{
	}

	// This ctor should be used only by business and should remain internal.
	// To update a ToDoTask, use the "with" operator.
	internal ToDoTask(string listId, TaskData data)
	{
		ListId = listId;

		Id = data.Id ?? throw new ArgumentNullException("data.Id", "Task must have a valid ID."); ;
		Importance = data.Importance;
		IsReminderOn = data.IsReminderOn;
		Status = data.Status;
		Title = data.Title;
		CreatedDateTime = data.CreatedDateTime;
		LastModifiedDateTime = data.LastModifiedDateTime;
		Body = data.Body;
		DueDateTime = data.DueDateTime;
		LinkedResourcesOdataContext = data.LinkedResourcesOdataContext;
		LinkedResources = data.LinkedResources?.ToImmutableList();
		OdataEtag = data.OdataEtag;
	}

	public string ListId { get; } // No public init: this can be set only from a data

	public string Id { get; } // No public init: this can be set only from a data

	public string? Importance { get; init; }

	public bool IsReminderOn { get; init; }

	public string? Status { get; init; }

	public string? Title { get; init; }

	public DateTime CreatedDateTime { get; init; }

	public DateTime LastModifiedDateTime { get; init; }

	public TaskBodyData? Body { get; init; }

	public DateTimeData? DueDateTime { get; init; }

	public string? LinkedResourcesOdataContext { get; init; }

	public ImmutableList<LinkedResourceData>? LinkedResources { get; init; }

	public string? OdataEtag { get; init; }

	// This ctor should be used only by business and should remain internal.
	[Pure]
	internal TaskData ToData()
		=> new()
		{
			Id = Id,
			Importance = Importance,
			IsReminderOn = IsReminderOn,
			Status = Status,
			Title = Title,
			CreatedDateTime = CreatedDateTime,
			LastModifiedDateTime = LastModifiedDateTime,
			Body = Body,
			DueDateTime = DueDateTime,
			LinkedResourcesOdataContext = LinkedResourcesOdataContext,
			LinkedResources = LinkedResources?.ToList(),
			OdataEtag = OdataEtag,
		};
}

namespace ToDo;

public record CheckListItem
{
	internal CheckListItem()
	{
		Id = default!;
	}

	internal CheckListItem(CheckListItemData data)
	{
		Id = data.Id ?? throw new ArgumentNullException("data.Id", "CheckListItem must have a valid ID.");
		DisplayName = data.DisplayName;
		IsChecked = data.IsChecked;
		CreatedDateTime = data.CreatedDateTime;
		CheckedDateTime = data.CheckedDateTime;
	}

	public string Id { get; }

	public bool IsChecked { get; init; }

	public string? DisplayName { get; init; }

	public DateTime CreatedDateTime { get; init; }

	public DateTime? CheckedDateTime { get; init; }

	// This ctor should be used only by business and should remain internal.
	[Pure]
	internal CheckListItemData ToData()
		=> new()
		{
			Id = Id,
			CheckedDateTime = CheckedDateTime,
			IsChecked = IsChecked,
			CreatedDateTime = CreatedDateTime,
			DisplayName = DisplayName
		};
}

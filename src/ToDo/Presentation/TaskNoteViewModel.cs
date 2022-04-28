namespace ToDo.Presentation;

public partial class TaskNoteViewModel
{
	private readonly INavigator _navigator;

	public string? Note { get; set; }

	public TaskNoteViewModel(
		INavigator navigator,
		ICommandBuilder addNote)
	{
		_navigator = navigator;
		addNote.Execute(AddNote);
	}

	private async ValueTask AddNote(CancellationToken ct)
	{
		await _navigator.NavigateBackWithResultAsync(this, data: new TaskBodyData { Content = Note });
	}
}

namespace ToDo.Presentation;

public partial class TaskNoteViewModel
{
	private INavigator Navigator;

	public string? Note { get; set; }

	public TaskNoteViewModel(
		INavigator navigator,
		ICommandBuilder addNote)
	{
		Navigator = navigator;
		addNote.Execute(AddNote);
	}

	private async ValueTask AddNote(CancellationToken ct)
	{
		await Navigator.NavigateBackWithResultAsync(this, data: new TaskBodyData { Content = Note });
	}
}

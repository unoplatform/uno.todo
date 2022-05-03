namespace ToDo.Presentation;

public partial class TaskNoteViewModel
{
	private readonly INavigator _navigator;

	public string? Note { get; set; }

	public TaskNoteViewModel(INavigator navigator)
	{
		_navigator = navigator;
	}

	public ICommand AddNote => Command.Async(DoAddNote);
	private async ValueTask DoAddNote(CancellationToken ct)
	{
		await _navigator.NavigateBackWithResultAsync(this, data: new TaskBodyData { Content = Note }, cancellation: ct);
	}
}

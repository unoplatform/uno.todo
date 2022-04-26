namespace ToDo.Presentation;

public partial class TaskNoteViewModel
{
	private INavigator Navigator { get; }

	public ICommand AddNoteCommand { get; }

	public string? Note { get; set; }

	public TaskNoteViewModel(
		INavigator navigator)
	{

		Navigator = navigator;

		AddNoteCommand = new AsyncRelayCommand(AddNote);
	}

	public async Task AddNote()
	{
		await Navigator.NavigateBackWithResultAsync(this, data: new TaskBodyData { Content = Note });
	}
}

namespace ToDo.Views;

public sealed partial class TaskListPage : Page, IInjectable<INavigator>
{
	public TaskListPage()
	{
		this.InitializeComponent();
	}

	public async void CreateTaskClick(object sender, RoutedEventArgs args)
	{

		var response = await Navigator!.NavigateViewModelForResultAsync<AddTaskViewModel, TaskData>(this, qualifier: Qualifiers.Dialog);
		if (response is null)
		{
			return;
		}

		var result = await response.Result;

		var taskTitle = result.SomeOrDefault()?.Title;

	}

	public async void DeleteListClick(object sender, RoutedEventArgs args)
	{

		var response = await Navigator!.NavigateRouteForResultAsync< DialogAction>(this, "ConfirmDeleteList", qualifier: Qualifiers.Dialog);
		if (response is null)
		{
			return;
		}

		var result = await response.Result;

		var actionLabel= result.SomeOrDefault()?.Label;

	}

	private INavigator? Navigator { get; set; }
	public void Inject(INavigator entity) => Navigator = entity;
}


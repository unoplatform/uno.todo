
namespace ToDo.Views;

public sealed partial class HomePage : Page
{
	public HomePage()
	{
		this.InitializeComponent();
		this.Loaded += (s, e) => NavView.IsPaneOpen = true;
	}

	public async void CreateTaskListClick(object sender, RoutedEventArgs args)
	{
		var response = await Navigator!.NavigateViewModelForResultAsync<AddListViewModel, TaskListRequestData>(this, qualifier: Qualifiers.Dialog);
		if (response is null)
		{
			return;
		}

		var result = await response.Result;

		var listName = result.SomeOrDefault()?.DisplayName;

	}

	private void OpenNavPane(object sender, RoutedEventArgs e)
	{
		NavView.IsPaneOpen = true;
	}

	private INavigator? Navigator { get; set; }
	public void Inject(INavigator entity) => Navigator = entity;
}

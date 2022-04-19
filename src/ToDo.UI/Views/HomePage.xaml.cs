
namespace ToDo.Views;

public sealed partial class HomePage : Page
{
    public HomePage()
    {
        this.InitializeComponent();
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

	private INavigator? Navigator { get; set; }
	public void Inject(INavigator entity) => Navigator=entity;
}

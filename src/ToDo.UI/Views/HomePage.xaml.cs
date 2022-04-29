namespace ToDo.Views;

public sealed partial class HomePage : Page
{
	public HomePage()
	{
		this.InitializeComponent();

		// keep nav-view open when resizing past the threshold
		// note: we cant keep nav-view open on uwp when changing PaneDisplayMode or using the auto mode with threshold properties.
		this.Loaded += (s, e) => NavView.IsPaneOpen = true;
		NavView.SizeChanged += (s, e) =>
		{
			var previousMode = NavView.PaneDisplayMode;
			var newMode = e.NewSize.Width > 800
				? NavigationViewPaneDisplayMode.Left
				: NavigationViewPaneDisplayMode.LeftCompact;
			if (newMode != previousMode)
			{
				NavView.PaneDisplayMode = newMode;
				NavView.IsPaneOpen = true;
			}
		};
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

	private void ToggleNavPane(object sender, RoutedEventArgs e)
	{
		NavView.IsPaneOpen ^= true;
	}

	private INavigator? Navigator { get; set; }
	public void Inject(INavigator entity) => Navigator = entity;
}

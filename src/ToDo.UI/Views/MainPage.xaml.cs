
namespace ToDo.Views;

public sealed partial class MainPage : Page, IInjectable<INavigator>
{
	public MainViewModel? ViewModel { get; private set; }
	private INavigator? _navigator;

	public MainPage()
	{
		this.InitializeComponent();

		DataContextChanged += (_, changeArgs) => ViewModel = changeArgs?.NewValue as MainViewModel;

	}

	public void GoToSecondPageClick(object sender, RoutedEventArgs arg)
	{
		if (_navigator is not null)
		{
			_navigator.NavigateViewAsync<SecondPage>(this);
		}
	}

	public void Inject(INavigator navigator)
	{
		_navigator = navigator;
	}

}

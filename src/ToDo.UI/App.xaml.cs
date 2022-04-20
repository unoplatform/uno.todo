#pragma warning disable 109 // Remove warning for Window property on iOS

using ToDo.Business.Services;
using ToDo.Views.Dialogs;

namespace ToDo;

public sealed partial class App : Application
{
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
	private Window? _window;
	public new Window? Window => _window;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

	private IHost Host { get; }

	public App()
	{
		Host = UnoHost
				.CreateDefaultBuilder()
#if DEBUG
				// Switch to Development environment when running in DEBUG
				.UseEnvironment(Environments.Development)
#endif


				// Add platform specific log providers
				.UseLogging()

				// Configure log levels for different categories of logging
				.ConfigureLogging(logBuilder =>
				{
					logBuilder
							.SetMinimumLevel(LogLevel.Information)
							.XamlLogLevel(LogLevel.Information)
							.XamlLayoutLogLevel(LogLevel.Information);
				})

				// Load configuration information from appsettings.json
				.UseEmbeddedAppSettings<App>()
				.UseCustomEmbeddedSettings<App>("appsettings.platform.json")

				// Load AppInfo section
				.UseConfiguration<AppInfo>()
				.UseConfiguration<OAuthSettings>()


				.UseSettings<ToDoSettings>()


				// Register Json serializers (ISerializer and IStreamSerializer)
				.UseSerialization()

				// Register services for the application
				.ConfigureServices((context, services) =>
				{
					services
						.AddSingleton<IAuthenticationService, AuthenticationService>()
						.AddEndpoints(context, AcquireToken)
#if DEBUG // Comment these out if you want to use actual data from ToDo (requires auth)
						// TODO: Still need a way to dynamically toggle between mock and live endpoints
						.AddSingleton<ITaskListEndpoint, ToDo.Data.Mock.MockTaskListEndpoint>()
						.AddSingleton<ITaskEndpoint, ToDo.Data.Mock.MockTaskEndpoint>()
#endif
						.AddServices();
				})


				// Enable navigation, including registering views and viewmodels
				.UseNavigation(RegisterRoutes)

				// Add navigation support for toolkit controls such as TabBar and NavigationView
				.UseToolkitNavigation()


				.Build(enableUnoLogging: true);

		this.InitializeComponent();

#if HAS_UNO || NETFX_CORE
		this.Suspending += OnSuspending;
#endif
	}

	//private async Task<string> GetAccessToken()
	//{
	//	//TODO:There is a IAuthenticationService already to use it with injection
	//	if (_auth is null)
	//		throw new Exception("_auth is null");
	//	//We need to save authResult in order to consume it from HomeViewModel and show User's email and name
	//	var authResult = await _auth.ReturnAuthResultContext();
	//	return authResult.AccessToken;
	//}
	
	private async Task<string> AcquireToken(IServiceProvider services)
	{
		var auth = services.GetRequiredService<IAuthenticationService>();
		var authResult = await auth.ReturnAuthResultContext();
		if (authResult.AccessToken is not null)
		{
			return authResult.AccessToken;
		}

		return string.Empty;
	
	}

	/// <summary>
	/// Invoked when the application is launched normally by the end user.  Other entry points
	/// will be used such as when the application is launched to open a specific file.
	/// </summary>
	/// <param name="args">Details about the launch request and process.</param>
	protected async override void OnLaunched(LaunchActivatedEventArgs args)
	{
#if DEBUG
		if (System.Diagnostics.Debugger.IsAttached)
		{
			// this.DebugSettings.EnableFrameRateCounter = true;
		}
#endif

#if NET5_0 && WINDOWS
		_window = new Window();
		_window.Activate();
#else
#if WINUI
		_window = Microsoft.UI.Xaml.Window.Current;
#else
		_window = Windows.UI.Xaml.Window.Current;
#endif
#endif

		if (Host.Services.GetService<IRouteNotifier>() is { } notif)
		{
			notif.RouteChanged += RouteUpdated;
		}

		_window.AttachNavigation(Host.Services);
		_window.Activate();

		await System.Threading.Tasks.Task.Run(async () =>
		{
			await Host.StartAsync();
		});

	}

	/// <summary>
	/// Invoked when Navigation to a certain page fails
	/// </summary>
	/// <param name="sender">The Frame which failed navigation</param>
	/// <param name="e">Details about the navigation failure</param>
	void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
	{
		throw new InvalidOperationException($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
	}

	/// <summary>
	/// Invoked when application execution is being suspended.  Application state is saved
	/// without knowing whether the application will be terminated or resumed with the contents
	/// of memory still intact.
	/// </summary>
	/// <param name="sender">The source of the suspend request.</param>
	/// <param name="e">Details about the suspend request.</param>
	private void OnSuspending(object sender, SuspendingEventArgs e)
	{
		var deferral = e.SuspendingOperation.GetDeferral();
		// TODO: Save application state and stop any background activity
		deferral.Complete();
	}

	private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
	{
		var confirmDialog = new MessageDialogViewMap(
		Content: "Are you sure you want to delete?",
		Title: "Confirm delete?",
		DelayUserInput: true,
		DefaultButtonIndex: 1,
		Buttons: new DialogAction[]
		{
						new(Label: "Yeh!",Id:"Y"),
						new(Label: "Nah", Id:"N")
		}
	);


		views.Register(
			new ViewMap<ShellControl, ShellViewModel>(),
			new ViewMap<WelcomePage, WelcomeViewModel>(),
			new ViewMap<HomePage, HomeViewModel.BindableHomeViewModel>(),
			new ViewMap<TaskListPage, TaskListViewModel.BindableTaskListViewModel>(),
			new ViewMap<TaskPage, TaskViewModel.BindableTaskViewModel>(),
			new ViewMap<AddTaskDialog>(),
			new ViewMap<AddListDialog, AddListViewModel>(),
			new ViewMap<AuthTokenDialog, AuthTokenViewModel>(),
			confirmDialog
			);

		routes
			.Register(
			views =>
				new("", View: views.FindByViewModel<ShellViewModel>(),
						Nested: new RouteMap[]
						{
							new ("Welcome",
									View: views.FindByViewModel<WelcomeViewModel>()
									),
							new ("TaskLists",
									View: views.FindByViewModel<HomeViewModel.BindableHomeViewModel>()
									),
							new("TaskList",
									View: views.FindByViewModel<TaskListViewModel.BindableTaskListViewModel>(),
									DependsOn:"TaskLists"),
							new("Task",
									View: views.FindByViewModel<TaskViewModel.BindableTaskViewModel>(),
									DependsOn:"TaskLists"),
							new("AddTask",
								View: views.FindByView<AddTaskDialog>()),
							new("AddList",
								View: views.FindByViewModel<AddListViewModel>()),
							new("AuthToken",
								View: views.FindByViewModel<AuthTokenViewModel>()),
							new ("Confirm", confirmDialog)
						}));
	}

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
	public void RouteUpdated(object? sender, RouteChangedEventArgs e)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
	{
		try
		{
			var rootRegion = e.Region.Root();
			var route = rootRegion.GetRoute();


#if !__WASM__ && !WINUI
			CoreApplication.MainView?.DispatcherQueue.TryEnqueue(() =>
			{
				var appTitle = ApplicationView.GetForCurrentView();
				appTitle.Title = "ToDo: " + (route + "").Replace("+", "/");
			});
#endif

		}
		catch (Exception ex)
		{
			Console.WriteLine("Error: " + ex.Message);
		}
	}
}

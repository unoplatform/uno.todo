#pragma warning disable 109 // Remove warning for Window property on iOS

using Uno.Extensions.Localization;
using Uno.Extensions.Navigation.UI;
using Windows.ApplicationModel.Resources;

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
		ChangeStartingLanguage();
		
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
							.XamlLayoutLogLevel(LogLevel.Information)
							.AddFilter("Uno.Extensions.Navigation", LogLevel.Trace);
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
						.AddEndpoints(context)
						.AddServices();
				})

				// Enable navigation, including registering views and viewmodels
				.UseNavigation(RegisterRoutes)

				// Add navigation support for toolkit controls such as TabBar and NavigationView
				.UseToolkitNavigation()

				// Add localization support
				.UseLocalization()

				.Build(enableUnoLogging: true);

		this.InitializeComponent();

#if HAS_UNO || NETFX_CORE
		this.Suspending += OnSuspending;
#endif
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
		var res = ResourceLoader.GetForViewIndependentUse();
		
		MessageDialogViewMap BuildDialogViewMap(string section, bool delayUserInput, int defaultButtonIndex, params (object Id, string labelKeyPath)[] buttons)
		{
			return new MessageDialogViewMap
			(
				Content: ResLookup("Content"),
				Title: ResLookup("Title"),
				DelayUserInput: delayUserInput,
				DefaultButtonIndex: defaultButtonIndex,
				Buttons: buttons
					.Select(x => new DialogAction(Label: ResLookup(x.labelKeyPath), Id: x.Id))
					.ToArray()
			);
			string ResLookup(string keyPath)
			{
				// map absolute/relative path accordingly
				var key = keyPath.StartsWith("./") ? keyPath.Substring(2) : $"Dialog_{section}_{keyPath}";
				return res!.GetString(key);
			}
		}

		var deleteButton = (DialogResults.Affirmative, "./Dialog_Common_Delete");
		var cancelButton = (DialogResults.Negative, "./Dialog_Common_Cancel");
		var confirmDeleteListDialog = BuildDialogViewMap("ConfirmDeleteList", true, 0, deleteButton, cancelButton);
		var confirmDeleteTaskDialog = BuildDialogViewMap("ConfirmDeleteTask", true, 0, deleteButton, cancelButton);
		var confirmDeleteNoteDialog = BuildDialogViewMap("ConfirmDeleteNote", true, 0, deleteButton, cancelButton);
		var confirmSignOutDialog = BuildDialogViewMap("ConfirmSignOut", true, 0, (DialogResults.Affirmative, "SignOut"), cancelButton);

		views.Register(
			/// Dialogs and Flyouts
			new ViewMap<AddTaskFlyout, AddTaskViewModel>(),
			new ViewMap<AddListFlyout, AddListViewModel>(),
			new ViewMap<AuthTokenDialog, AuthTokenViewModel>(),
			new ViewMap<ExpirationDateFlyout, ExpirationDateViewModel>(),
			new ViewMap<RenameListFlyout, RenameListViewModel>(),

			// Views
			new ViewMap<HomePage, HomeViewModel.BindableHomeViewModel>(),
			new ViewMap<TaskSearchFlyout>(),
			new ViewMap<SearchPage, SearchViewModel.BindableSearchViewModel>(),
			new ViewMap<SettingsPage, SettingsViewModel.BindableSettingsViewModel>(),
			new ViewMap<ShellControl, ShellViewModel>(),
			new ViewMap<WelcomePage, WelcomeViewModel.BindableWelcomeViewModel>(),
			new ViewMap<TaskListPage, TaskListViewModel.BindableTaskListViewModel>(Data: new DataMap<TaskList>()),
			new ViewMap(
				DynamicView: () => (App.Current as App)?.Window?.Content?.ActualSize.X > 1000 ? typeof(TaskControl) : typeof(TaskPage),
				ViewModel: typeof(TaskViewModel.BindableTaskViewModel), Data: new DataMap<ToDoTask>()),
			new ViewMap<AuthTokenDialog, AuthTokenViewModel>(),
			confirmDeleteListDialog,
			confirmDeleteTaskDialog,
			confirmDeleteNoteDialog,
			confirmSignOutDialog
		);

		routes.Register(
			new RouteMap("", View: views.FindByViewModel<ShellViewModel>(), Nested: new RouteMap[]
			{
				new("Welcome", View: views.FindByViewModel<WelcomeViewModel.BindableWelcomeViewModel>()),
				new("Home", View: views.FindByViewModel<HomeViewModel.BindableHomeViewModel>()),
				new("TaskList", View: views.FindByViewModel<TaskListViewModel.BindableTaskListViewModel>(), Nested: new[]
				{
					new RouteMap("MultiTaskLists", IsDefault: true, Nested: new[]
					{
						new RouteMap("ToDo", IsDefault:true),
						new RouteMap("Completed")
					})
				}),
				new("Task", View: views.FindByViewModel<TaskViewModel.BindableTaskViewModel>(), DependsOn:"TaskList"),
				new("TaskSearch", View: views.FindByView<TaskSearchFlyout>(), Nested: new RouteMap[]
				{
					new("Search", View: views.FindByViewModel<SearchViewModel.BindableSearchViewModel>(), IsDefault: true)
				}),
				new("Settings", View: views.FindByViewModel<SettingsViewModel.BindableSettingsViewModel>()),
				new("TaskNote", View: views.FindByViewModel<TaskNoteViewModel>(), DependsOn:"Task"),
				new("AddTask", View: views.FindByView<AddTaskViewModel>()),
				new("AddList", View: views.FindByViewModel<AddListViewModel>()),
				new("AuthToken", View: views.FindByViewModel<AuthTokenViewModel>()),
				new("ExpirationDate", View: views.FindByViewModel<ExpirationDateViewModel>()),
				new("RenameList", View: views.FindByViewModel<RenameListViewModel>()),
				new("ConfirmDeleteList", confirmDeleteListDialog),
				new("ConfirmDeleteTask", confirmDeleteTaskDialog),
				new("ConfirmDeleteNote", confirmDeleteNoteDialog),
				new("ConfirmSignOut", confirmSignOutDialog)
			})
		);
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

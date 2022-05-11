#if DEBUG
#define USE_MOCKS
#endif

#pragma warning disable 109 // Remove warning for Window property on iOS

using ToDo.Extensions;

namespace ToDo;

public sealed partial class App : Application
{
	private const string _mock = "mock";
	private readonly IHost _host = BuildAppHost();

	private static IHost BuildAppHost()
	{
#if USE_MOCKS
		var useMocks = true;
#else
		var useMocks = false;
#endif

#if __WASM__
		var stringUri = WebAssemblyRuntime.InvokeJS("window.location.href;");
		var query = new Uri(stringUri).Query;
		var queriesValues = System.Web.HttpUtility.ParseQueryString(query);

		if (queriesValues.TryGetValue(_mock, out var value) && bool.TryParse(value, out var isMocked))
		{
			useMocks = isMocked;
		}
#endif
		return UnoHost
				.CreateDefaultBuilder()
#if DEBUG
				// Switch to Development environment when running in DEBUG
				.UseEnvironment(Environments.Development)
#endif

				// Add platform specific log providers
				//Note for Extensions, at the moment we have the error CS0121: The call is ambiguous between the following methods or properties: 'HostBuilderExtensions.UseLogging(IHostBuilder, Action<ILoggingBuilder>?)' and 'HostBuilderExtensions.UseLogging(IHostBuilder, Action<HostBuilderContext, ILoggingBuilder>?)'
				.UseLogging(_ => { })

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

				// Load OAuth configuration
				.UseConfiguration<Auth>()

				// Enable app settings
				.UseSettings<ToDoApp>()

				// Register Json serializers (ISerializer and IStreamSerializer)
				.UseSerialization()

				// Register services for the application
				.ConfigureServices(
					(context, services) =>
						services
							.AddScoped<IAppTheme, AppTheme>()
							.AddEndpoints(context, useMocks: useMocks)
							.AddServices(useMocks: useMocks)
						)

				// Enable navigation, including registering views and viewmodels
				.UseNavigation(
						RegisterRoutes,
						createViewRegistry: sc => new ReactiveViewRegistry(sc, ReactiveViewModelMappings.ViewModelMappings))
					.ConfigureServices(services =>
					{
						services
							.AddSingleton<IRouteResolver, ReactiveRouteResolver>();
					})

				// Add navigation support for toolkit controls such as TabBar and NavigationView
				.UseToolkitNavigation()

				// Add localization support
				.UseLocalization()

				.Build(enableUnoLogging: true);
	}

	private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
	{
		LocalizableMessageDialogViewMap BuildDialogViewMap(string section, bool delayUserInput, int defaultButtonIndex, params (object Id, string labelKeyPath)[] buttons)
		{
			return new LocalizableMessageDialogViewMap
			(
				Content: localizer => localizer![ResourceKey(ResourceKeys.DialogContent)],
				Title: localizer => localizer![ResourceKey(ResourceKeys.DialogTitle)],
				DelayUserInput: delayUserInput,
				DefaultButtonIndex: defaultButtonIndex,
				Buttons: buttons
					.Select(x => new LocalizableDialogAction(LabelProvider: localizer => localizer![ResourceKey(x.labelKeyPath)], Id: x.Id))
					.ToArray()
			);
			string ResourceKey(string keyPath)
			{
				// map absolute/relative path accordingly
				return keyPath.StartsWith("./") ? keyPath.Substring(2) : $"Dialog_{section}_{keyPath}";
			}
		}

		var deleteButton = (DialogResults.Affirmative, ResourceKeys.DeleteButton);
		var cancelButton = (DialogResults.Negative, ResourceKeys.CancelButton);
		var confirmDeleteListDialog = BuildDialogViewMap(Dialog.ConfirmDeleteList, true, 0, deleteButton, cancelButton);
		var confirmDeleteTaskDialog = BuildDialogViewMap(Dialog.ConfirmDeleteTask, true, 0, deleteButton, cancelButton);
		var confirmSignOutDialog = BuildDialogViewMap(Dialog.ConfirmSignOut, true, 0, (DialogResults.Affirmative, ResourceKeys.SignOutButton), cancelButton);

		views.Register(
			/// Dialogs and Flyouts
			new ViewMap<AddTaskFlyout, AddTaskViewModel>(),
			new ViewMap<AddListFlyout, AddListViewModel>(),
			new ViewMap<ExpirationDateFlyout, ExpirationDateViewModel>(),
			new ViewMap<RenameListFlyout, RenameListViewModel>(),

			// Views
			new ViewMap<HomePage, HomeViewModel>(),
			new ViewMap<TaskSearchFlyout>(),
			new ViewMap<SearchPage, SearchViewModel>(),
			new ViewMap<SettingsFlyout, SettingsViewModel>(),
			new ViewMap<ShellControl, ShellViewModel>(),
			new ViewMap<WelcomePage, WelcomeViewModel>(),
			new ViewMap<TaskListPage, TaskListViewModel>(Data: new DataMap<TaskList>()),
			new ViewMap(
				ViewSelector: () => (App.Current as App)?.Window?.Content?.ActualSize.X > (double)App.Current.Resources[ResourceKeys.WideMinWindowWidth] ? typeof(TaskControl) : typeof(TaskPage),
				ViewModel: typeof(TaskViewModel), Data: new DataMap<ToDoTask>()),
			confirmDeleteListDialog,
			confirmDeleteTaskDialog,
			confirmSignOutDialog
		);

		routes.Register(
			new RouteMap("", View: views.FindByViewModel<ShellViewModel>(), Nested: new RouteMap[]
			{
				new("Welcome", View: views.FindByViewModel<WelcomeViewModel>()),
				new("Home", View: views.FindByViewModel<HomeViewModel>()),
				new("TaskList", View: views.FindByViewModel<TaskListViewModel>(), Nested: new[]
				{
					new RouteMap("MultiTaskLists", IsDefault: true, Nested: new[]
					{
						new RouteMap("ToDo", IsDefault:true),
						new RouteMap("Completed")
					})
				}),
				new("Task", View: views.FindByViewModel<TaskViewModel>(), DependsOn:"TaskList"),
				new("TaskSearch", View: views.FindByView<TaskSearchFlyout>(), Nested: new RouteMap[]
				{
					new("Search", View: views.FindByViewModel<SearchViewModel>(), IsDefault: true)
				}),
				new("Settings", View: views.FindByViewModel<SettingsViewModel>()),
				new("AddTask", View: views.FindByViewModel<AddTaskViewModel>()),
				new("AddList", View: views.FindByViewModel<AddListViewModel>()),
				new("ExpirationDate", View: views.FindByViewModel<ExpirationDateViewModel>()),
				new("RenameList", View: views.FindByViewModel<RenameListViewModel>()),
				new(Dialog.ConfirmDeleteList, confirmDeleteListDialog),
				new(Dialog.ConfirmDeleteTask, confirmDeleteTaskDialog),
				new(Dialog.ConfirmSignOut, confirmSignOutDialog)
			})
		);
	}
}


// Only define mocks in debug as we don't have a way to dynamically switch them
#if DEBUG
#define USE_MOCKS
#endif

#pragma warning disable 109 // Remove warning for Window property on iOS

using ToDo.Business.Models;
using ToDo.Configuration;

namespace ToDo;

public sealed partial class App : Application
{
	private readonly IHost _host = BuildAppHost();

	private static IHost BuildAppHost()
	{
#if USE_MOCKS
		var useMocks = true;
#else
		var useMocks = false;
#endif

		return UnoHost
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

				// Load OAuth configuration
				.UseConfiguration<Auth>()

				// Register Json serializers (ISerializer and IStreamSerializer)
				.UseSerialization()

				// Register services for the application
				.ConfigureServices(
					(context, services) =>
						services
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
				Content: localizer => localizer![ResourceKey("Content")],
				Title: localizer => localizer![ResourceKey("Title")],
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
				ViewSelector: () => (App.Current as App)?.Window?.Content?.ActualSize.X > (double)App.Current.Resources["WideMinWindowWidth"] ? typeof(TaskControl) : typeof(TaskPage),
				ViewModel: typeof(TaskViewModel.BindableTaskViewModel), Data: new DataMap<ToDoTask>()),
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
				new("AddTask", View: views.FindByViewModel<AddTaskViewModel>()),
				new("AddList", View: views.FindByViewModel<AddListViewModel>()),
				new("ExpirationDate", View: views.FindByViewModel<ExpirationDateViewModel>()),
				new("RenameList", View: views.FindByViewModel<RenameListViewModel>()),
				new("ConfirmDeleteList", confirmDeleteListDialog),
				new("ConfirmDeleteTask", confirmDeleteTaskDialog),
				new("ConfirmDeleteNote", confirmDeleteNoteDialog),
				new("ConfirmSignOut", confirmSignOutDialog)
			})
		);
	}
}

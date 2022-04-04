#pragma warning disable 109 // Remove warning for Window property on iOS

using Microsoft.Extensions.Http;
using Refit;
using Uno.Extensions.Http;
using Uno.Extensions.Http.Refit;
using System.Net.Http;
using Uno.Extensions.Serialization.Refit;

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

                // Load AppInfo section
                .UseConfiguration<AppInfo>()

                // Register Json serializers (ISerializer and IStreamSerializer)
                .UseSerialization()!

                // Register services for the application
                .ConfigureServices((context,services)=>
                {
                    services.AddEndpoints(context, GetAccessToken);
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

    private Task<string> GetAccessToken()
    {
        return Task.FromResult("TBD: Need to connect in MSAL to provide access token when requeested");
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

        _window.Content = Host.Services.NavigationHost();
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
        views.Register(
            new ViewMap<ShellControl, ShellViewModel>(),
            new ViewMap<MainPage, MainViewModel>(),
            new ViewMap<SecondPage, SecondViewModel>()
            );

        routes
            .Register(
            views =>
                new("", View: views.FindByViewModel<ShellViewModel>(),
                        Nested: new RouteMap[]
                        {
                                        new ("Main",
                                                View: views.FindByViewModel<MainViewModel>(),
                                                IsDefault: true
                                                ),
                                        new("Second",
                                                View: views.FindByViewModel<SecondViewModel>(),
                                                DependsOn:"Main"),
                        }));
    }

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    public async void RouteUpdated(object? sender, RouteChangedEventArgs e)
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

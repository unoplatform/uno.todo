// Only define mocks in debug as we don't have a way to dynamically switch them
#if DEBUG
//#define USE_MOCKS
#endif

using ToDo.Business.Services;

namespace ToDo;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddEndpoints(
		this IServiceCollection services,
		HostBuilderContext context,
		Action<IServiceProvider, RefitSettings>? settingsBuilder = null)
	{
		return services
			.AddNativeHandler()
			.AddContentSerializer()
			.AddRefitClient<ITaskEndpoint>(context, nameof(ITaskEndpoint), settingsBuilder)
			.AddRefitClient<ITaskListEndpoint>(context, nameof(ITaskEndpoint), settingsBuilder)

// Comment out the USE_MOCKS definition (top of this file) to prevent using mocks in development
#if USE_MOCKS
			.AddSingleton<ITaskListEndpoint, ToDo.Data.Mock.MockTaskListEndpoint>()
			.AddSingleton<ITaskEndpoint, ToDo.Data.Mock.MockTaskEndpoint>()
#endif
			;
	}

	public static IServiceCollection AddServices(this IServiceCollection services)
	=> services
		.AddSingleton<ITaskService, TaskService>()
		.AddSingleton<ITaskListService, TaskListService>()
		.AddSingleton<IAuthenticationService, AuthenticationService>()
		.AddSingleton<IAuthenticationTokenProvider>(sp => sp.GetRequiredService<IAuthenticationService>())
		.AddSingleton<IMessenger, WeakReferenceMessenger>()

		// Comment out the USE_MOCKS definition (top of this file) to prevent using mocks in development
#if USE_MOCKS
		.AddSingleton<IAuthenticationService, MockAuthenticationService>()
#endif
		;
}

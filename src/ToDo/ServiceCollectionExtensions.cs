namespace ToDo;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddEndpoints(
		this IServiceCollection services,
		HostBuilderContext context,
		Action<IServiceProvider, RefitSettings>? settingsBuilder = null,
		bool useMocks=false)
	{
		_ = services
			.AddNativeHandler()
			.AddContentSerializer()
			.AddRefitClient<ITaskEndpoint>(context, nameof(ITaskEndpoint), settingsBuilder)
			.AddRefitClient<ITaskListEndpoint>(context, nameof(ITaskEndpoint), settingsBuilder);

		if (useMocks)
		{
			services.AddSingleton<ITaskListEndpoint, ToDo.Data.Mock.MockTaskListEndpoint>()
			.AddSingleton<ITaskEndpoint, ToDo.Data.Mock.MockTaskEndpoint>();
		}
		return services;
	}

	public static IServiceCollection AddServices(
		this IServiceCollection services,
		bool useMocks = false)
	{
		_ = services
		   .AddSingleton<ITaskService, TaskService>()
		   .AddSingleton<ITaskListService, TaskListService>()
		   .AddSingleton<IAuthenticationService, AuthenticationService>()
		   .AddSingleton<IAuthenticationTokenProvider>(sp => sp.GetRequiredService<IAuthenticationService>())
		   .AddSingleton<IMessenger, WeakReferenceMessenger>();
		if (useMocks)
		{
			// Comment out the USE_MOCKS definition (top of this file) to prevent using mocks in development
			services.AddSingleton<IAuthenticationService, MockAuthenticationService>();
		}
		return services;
	}
}

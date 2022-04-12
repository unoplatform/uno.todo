

namespace ToDo;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddEndpoints(
		this IServiceCollection services,
		HostBuilderContext context,
		Func<IServiceProvider, Task<string>> accessTokenCallback)
	{
		Action<IServiceProvider, RefitSettings> authSettingsBuilder = (sp, settings) =>
		{
			settings.AuthorizationHeaderValueGetter = () => accessTokenCallback(sp);
		};

		return services
			.AddNativeHandler()
			.AddContentSerializer()
			.AddRefitClient<ITaskEndpoint>(context, nameof(ITaskEndpoint), settingsBuilder: authSettingsBuilder)
			.AddRefitClient<ITaskListEndpoint>(context, nameof(ITaskEndpoint), settingsBuilder: authSettingsBuilder);
	}

	public static IServiceCollection AddServices(this IServiceCollection services)
		=> services
			.AddSingleton<ITaskService, TaskService>()
			.AddSingleton<ITaskListService, TaskListService>()
			.AddSingleton<CommunityToolkit.Mvvm.Messaging.IMessenger, CommunityToolkit.Mvvm.Messaging.WeakReferenceMessenger>();
}



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


	/*********************** Temporary extensions until Uno.Extensions.Http is updated **************/
	// See: https://github.com/unoplatform/uno.extensions/pull/347

	//private static char[] InterfaceNamePrefix = new[] { 'i', 'I' };

	//public static IServiceCollection AddClient<TClient, TImplementation>(
	//		this IServiceCollection services,
	//		HostBuilderContext context,
	//		string? name = null,
	//		Func<IHttpClientBuilder, EndpointOptions, IHttpClientBuilder>? configure = null
	//	)
	//		where TClient : class
	//		where TImplementation : class, TClient
	//{
	//	Func<IServiceCollection, HostBuilderContext, IHttpClientBuilder> httpClientFactory = (s, c) => s.AddHttpClient<TClient, TImplementation>();

	//	return services.AddClient<TClient>(context, name, httpClientFactory, configure);
	//}

	//public static IServiceCollection AddClient<TClient, TImplementation>(
	//	this IServiceCollection services,
	//	HostBuilderContext context,
	//	EndpointOptions options,
	//	string? name = null,
	//	Func<IHttpClientBuilder, EndpointOptions, IHttpClientBuilder>? configure = null
	//)
	//	where TClient : class
	//	where TImplementation : class, TClient
	//{
	//	Func<IServiceCollection, HostBuilderContext, IHttpClientBuilder> httpClientFactory = (s, c) => s.AddHttpClient<TClient, TImplementation>();

	//	return services.AddClient<TClient>(context, options, name, httpClientFactory, configure);
	//}

	//public static IServiceCollection AddClient<TInterface>(
	//	this IServiceCollection services,
	//	HostBuilderContext context,
	//	string? name = null,
	//	Func<IServiceCollection, HostBuilderContext, IHttpClientBuilder>? httpClientFactory = null,
	//	Func<IHttpClientBuilder, EndpointOptions, IHttpClientBuilder>? configure = null
	//)
	//	where TInterface : class
	//{
	//	name ??= typeof(TInterface).IsInterface ? typeof(TInterface).Name.TrimStart(InterfaceNamePrefix) : typeof(TInterface).Name;
	//	var options = Microsoft.Extensions.Configuration.ConfigurationBinder.Get<EndpointOptions>(context.Configuration.GetSection(name));//.Get<EndpointOptions>();
	//	return services.AddClient<TInterface>(context, options!, name, httpClientFactory, configure);
	//}

	//public static IServiceCollection AddClient<TInterface>(
	//	this IServiceCollection services,
	//	HostBuilderContext context,
	//	EndpointOptions options,
	//	string? name = null,
	//	Func<IServiceCollection, HostBuilderContext, IHttpClientBuilder>? httpClientFactory = null,
	//	Func<IHttpClientBuilder, EndpointOptions, IHttpClientBuilder>? configure = null
	//)
	//	where TInterface : class
	//{
	//	name ??= typeof(TInterface).IsInterface ? typeof(TInterface).Name.TrimStart(InterfaceNamePrefix) : typeof(TInterface).Name;

	//	if (httpClientFactory is null)
	//	{
	//		httpClientFactory = (s, c) => s.AddHttpClient(name);
	//	}

	//	var httpClientBuilder = httpClientFactory(services, context);

	//	_ = httpClientBuilder
	//		.Conditional(
	//			options.UseNativeHandler,
	//			builder => builder.ConfigurePrimaryAndInnerHttpMessageHandler<HttpMessageHandler>())
	//		.ConfigureHttpClient((serviceProvider, client) =>
	//		{
	//			if (options.Url is not null)
	//			{
	//				client.BaseAddress = new Uri(options.Url);
	//			}
	//		})
	//		.Conditional(
	//			configure is not null,
	//			builder => configure?.Invoke(builder, options) ?? builder);
	//	return services;
	//}


	//public static IHttpClientBuilder ConfigurePrimaryAndInnerHttpMessageHandler<THandler>(this IHttpClientBuilder builder) where THandler : HttpMessageHandler
	//{
	//	if (builder == null)
	//	{
	//		throw new ArgumentNullException("builder");
	//	}

	//	builder.Services.Configure(builder.Name, delegate (HttpClientFactoryOptions options)
	//	{
	//		options.HttpMessageHandlerBuilderActions.Add(delegate (HttpMessageHandlerBuilder b)
	//		{
	//			var innerHandler = b.Services.GetRequiredService<THandler>() as HttpMessageHandler;
	//			if (b.PrimaryHandler is DelegatingHandler delegatingHandler)
	//			{
	//				delegatingHandler.InnerHandler = innerHandler;
	//				innerHandler = delegatingHandler;
	//			}

	//			b.PrimaryHandler = innerHandler;
	//		});
	//	});
	//	return builder;
	//}


	//public static IServiceCollection AddRefitClient<TInterface>(
	//	this IServiceCollection services,
	//	HostBuilderContext context,
	//	string? name = null,
	//	Action<RefitSettings>? settingsBuilder = null,
	//	Func<IHttpClientBuilder, EndpointOptions, IHttpClientBuilder>? configure = null
	//)
	//	where TInterface : class
	//{
	//	return services.AddClient<TInterface>(
	//		context,
	//		name,
	//		(s, c) => Refit.HttpClientFactoryExtensions.AddRefitClient<TInterface>(s, settingsAction: serviceProvider =>
	//		{
	//			var settings = new RefitSettings()
	//			{
	//				ContentSerializer = serviceProvider.GetRequiredService<IHttpContentSerializer>(),
	//			};
	//			settingsBuilder?.Invoke(settings);
	//			return settings;
	//		}),
	//		configure);
	//}
}

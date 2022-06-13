namespace ToDo;

// ********* Classes to be added to Reactive.Navigation ********* //
public class ReactiveViewRegistry : ViewRegistry
{
	public IDictionary<Type, Type> ViewModelMappings { get; }
	public ReactiveViewRegistry(IServiceCollection services, IDictionary<Type, Type> viewModelMappings) : base(services)
	{
		ViewModelMappings = viewModelMappings;
	}

	protected override void InsertItem(ViewMap item)
	{
		if (item.ViewModel is not null &&
			ViewModelMappings.TryGetValue(item.ViewModel, out var bindableViewModel))
		{
			item = new ReactiveViewMap(item.View, item.ViewSelector, item.ViewModel, item.Data, item.ResultData, bindableViewModel);
		}

		base.InsertItem(item);
	}
}

public class ReactiveRouteResolver : RouteResolver
{
	private readonly IDictionary<Type, Type> _viewModelMappings;
	public ReactiveRouteResolver(
		ILogger<ReactiveRouteResolver> logger,
		IRouteRegistry routes,
		ReactiveViewRegistry views) : base(logger, routes, views)
	{
		_viewModelMappings = views.ViewModelMappings;
	}

	protected override RouteInfo FromRouteMap(RouteMap drm)
	{
		var viewFunc = (drm.View?.View is not null) ?
										() => drm.View.View :
										drm.View?.ViewSelector;
		return base.FromRouteMap(drm) with {
			ViewModel = (drm.View is ReactiveViewMap rvmp) ? rvmp.BindableViewModel : drm.View?.ViewModel };
	}

	public override RouteInfo? FindByViewModel(Type? viewModelType)
	{
		if (viewModelType is not null &&
			_viewModelMappings.TryGetValue(viewModelType, out var bindableViewModel))
		{
			return base.FindByViewModel(bindableViewModel);
		}
		return base.FindByViewModel(viewModelType);
	}
}

public record ReactiveViewMap(
		Type? View = null,
		Func<Type?>? ViewSelector = null,
		Type? ViewModel = null,
		DataMap? Data = null,
		Type? ResultData = null,
		Type? BindableViewModel = null
	) : ViewMap(View, ViewSelector, ViewModel, Data, ResultData)
{
	public override void RegisterTypes(IServiceCollection services)
	{
		if (BindableViewModel is not null)
		{
			services.AddTransient(BindableViewModel);
		}

		base.RegisterTypes(services);
	}
}

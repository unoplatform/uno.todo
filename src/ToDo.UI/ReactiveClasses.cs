﻿namespace ToDo;

// Need an attribute to identify this as the static class where
// the ViewModelMappings should be source generated
// [ReactiveMappings]
public static partial class ReactiveViewModelMappings
{
}

// ********* Generated ********* //
public static partial class ReactiveViewModelMappings
{
	public static IDictionary<Type, Type> ViewModelMappings = new Dictionary<Type, Type>()
			{
				{ typeof(HomeViewModel), typeof(HomeViewModel.BindableHomeViewModel)},
				{ typeof(SearchViewModel),typeof(SearchViewModel.BindableSearchViewModel)},
				{ typeof(SettingsViewModel),typeof(SettingsViewModel.BindableSettingsViewModel)},
				{ typeof(WelcomeViewModel),typeof(WelcomeViewModel.BindableWelcomeViewModel)},
				{ typeof(TaskListViewModel),typeof(TaskListViewModel.BindableTaskListViewModel )},
				{ typeof(TaskViewModel),typeof(TaskViewModel.BindableTaskViewModel)}
			};

}
// ***************************** //



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
			item = new ReactiveViewMap(item.View, item.DynamicView, item.ViewModel, item.Data, item.ResultData, bindableViewModel);
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
										drm.View?.DynamicView;
		return new RouteInfo(
			Path: drm.Path,
			View: viewFunc,
			ViewAttributes: drm.View?.ViewAttributes,
			ViewModel: (drm.View is ReactiveViewMap rvmp)?rvmp.BindableViewModel: drm.View?.ViewModel,
			Data: drm.View?.Data?.Data,
			ToQuery: drm.View?.Data?.UntypedToQuery,
			FromQuery: drm.View?.Data?.UntypedFromQuery,
			ResultData: drm.View?.ResultData,
			IsDefault: drm.IsDefault,
			DependsOn: drm.DependsOn,
			Init: drm.Init,
			Nested: ResolveViewMaps(drm.Nested));
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
		Func<Type?>? DynamicView = null,
		Type? ViewModel = null,
		DataMap? Data = null,
		Type? ResultData = null,
		Type? BindableViewModel = null
	) : ViewMap(View, DynamicView, ViewModel, Data, ResultData)
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

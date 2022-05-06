﻿namespace ToDo.Presentation;

[ReactiveBindable]
public partial class WelcomeViewModel
{
	private readonly IAuthenticationService _authService;
	private readonly IUserProfilePictureService _userSvc;
	private readonly INavigator _navigator;
	private readonly IDispatcher _dispatcher;

	private WelcomeViewModel(
		IDispatcher dispatcher,
		INavigator navigator,
		IUserProfilePictureService userSvc,
		IAuthenticationService authService)
	{
		_dispatcher = dispatcher;
		_navigator =navigator;
		_authService = authService;
		_userSvc = userSvc;
	}

	public ICommand GetStarted => Command.Async(DoGetStarted);
	private async ValueTask DoGetStarted(CancellationToken ct)
	{
		var user = await _authService.AuthenticateAsync(_dispatcher, _userSvc, cancellation: ct);
		if (user is not null)
		{
			await _navigator.NavigateRouteAsync(this, string.Empty, cancellation: ct);
		}
	}
}

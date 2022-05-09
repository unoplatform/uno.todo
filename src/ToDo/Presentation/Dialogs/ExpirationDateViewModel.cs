namespace ToDo.Presentation.Dialogs;

[ReactiveBindable]
public partial class ExpirationDateViewModel
{
	private readonly INavigator _navigator;

	private ExpirationDateViewModel(INavigator navigator)
	{
		_navigator = navigator;
	}

	public ICommand Today => Command.Async(DoToday);
	private async ValueTask DoToday(CancellationToken ct)
	{
		await _navigator.NavigateBackWithResultAsync(this, data: new DateTimeData { DateTime = DateTime.Today }, cancellation: ct);
	}

	public ICommand Tomorrow => Command.Async(DoTomorrow);
	private async ValueTask DoTomorrow(CancellationToken ct)
	{
		await _navigator.NavigateBackWithResultAsync(this, data: new DateTimeData { DateTime = DateTime.Today.AddDays(1) }, cancellation: ct);
	}

	public ICommand NextWeek => Command.Async(DoNextWeek);
	private async ValueTask DoNextWeek(CancellationToken ct)
	{
		await _navigator.NavigateBackWithResultAsync(this, data: new DateTimeData { DateTime = DateTime.Today.AddDays(7) }, cancellation: ct);
	}

}

namespace ToDo.Presentation.Dialogs;

[ReactiveBindable]
public partial class ExpirationDateViewModel
{
	private readonly INavigator _navigator;

	public DateTime DueDate
	{
		get;
		set;//TODO: the idea is return to TaskViewModel with the DueDate value / call BackToTaskMethod
	}
	private ExpirationDateViewModel(INavigator navigator)
	{
		_navigator = navigator;
	}

	private async ValueTask BackToTask()
	{
		 await _navigator.NavigateBackWithResultAsync(this, data: new DateTimeData { DateTime = DueDate });
	}

	public ICommand SelectToday => Command.Async(DoSelectToday);
	private async ValueTask DoSelectToday(CancellationToken ct)
	{
		await _navigator.NavigateBackWithResultAsync(this, data: new DateTimeData { DateTime = DateTime.Today }, cancellation: ct);
	}

	public ICommand SelectTomorrow => Command.Async(DoSelectTomorrow);
	private async ValueTask DoSelectTomorrow(CancellationToken ct)
	{
		await _navigator.NavigateBackWithResultAsync(this, data: new DateTimeData { DateTime = DateTime.Today.AddDays(1) }, cancellation: ct);
	}

	public ICommand SelectNextWeek => Command.Async(DoSelectNextWeek);
	private async ValueTask DoSelectNextWeek(CancellationToken ct)
	{
		await _navigator.NavigateBackWithResultAsync(this, data: new DateTimeData { DateTime = DateTime.Today.AddDays(7) }, cancellation: ct);
	}

}

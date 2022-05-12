namespace ToDo.Presentation.Dialogs;

public partial class ExpirationDateViewModel
{
	private readonly INavigator _navigator;

	public DateTime DueDate
	{
		get;
		set;
	}
	private ExpirationDateViewModel(INavigator navigator)
	{
		_navigator = navigator;
		//DueDate.Execute(async (date, ct) => await _navigator.NavigateBackWithResultAsync(this, data: new DateTimeData { DateTime = date }, cancellation: ct);
	}

	public async ValueTask SelectToday(CancellationToken ct)
		=> await _navigator.NavigateBackWithResultAsync(this, data: new DateTimeData { DateTime = DateTime.Today }, cancellation: ct);

	public async ValueTask SelectTomorrow(CancellationToken ct)
		=> await _navigator.NavigateBackWithResultAsync(this, data: new DateTimeData { DateTime = DateTime.Today.AddDays(1) }, cancellation: ct);

	private async ValueTask DoSelectNextWeek(CancellationToken ct)
		=> await _navigator.NavigateBackWithResultAsync(this, data: new DateTimeData { DateTime = DateTime.Today.AddDays(7) }, cancellation: ct);

}

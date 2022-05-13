namespace ToDo.Presentation.Dialogs;

public partial class ExpirationDateViewModel
{
	private readonly INavigator _navigator;

	private ExpirationDateViewModel(INavigator navigator, DateTimeData entity)
	{
		_navigator = navigator;
		Entity = State.Value(this, () => entity);
		Entity.Execute(async (date, ct) =>
		{
			if (date is not null)
			{
				await _navigator.NavigateBackWithResultAsync(this, data: date, cancellation: ct);
			}
		});
	}

	public IState<DateTimeData> Entity { get; }

	public async ValueTask SelectToday(CancellationToken ct)
		=> await _navigator.NavigateBackWithResultAsync(this, data: new DateTimeData { DateTime = DateTime.Today }, cancellation: ct);

	public async ValueTask SelectTomorrow(CancellationToken ct)
		=> await _navigator.NavigateBackWithResultAsync(this, data: new DateTimeData { DateTime = DateTime.Today.AddDays(1) }, cancellation: ct);

	public async ValueTask SelectNextWeek(CancellationToken ct)
		=> await _navigator.NavigateBackWithResultAsync(this, data: new DateTimeData { DateTime = DateTime.Today.AddDays(7) }, cancellation: ct);

}

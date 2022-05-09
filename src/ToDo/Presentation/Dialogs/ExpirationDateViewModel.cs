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
}

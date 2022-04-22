public class Dispatcher : IDispatcher
{
	private readonly Window _window;
	public Dispatcher(Window window)
	{
		_window = window;
	}

	public Task Run(Func<Task> action) => _window.DispatcherQueue.Run(action);
	public Task<TResult> Run<TResult>(Func<Task<TResult>> actionWithResult) => _window.DispatcherQueue.Run(actionWithResult);
}

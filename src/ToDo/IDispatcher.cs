
namespace ToDo;

public interface IDispatcher
{
	Task Run(Func<Task> action);

	Task<TResult> Run<TResult>(Func<Task<TResult>> actionWithResult);
}

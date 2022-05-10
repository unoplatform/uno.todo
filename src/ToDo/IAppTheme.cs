namespace ToDo;

// TODO: Extract these to uno extensions
public interface IAppTheme
{
	bool IsDark { get; }

	void SetTheme(bool darkMode);
}

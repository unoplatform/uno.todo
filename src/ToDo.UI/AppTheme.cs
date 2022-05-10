namespace ToDo;

// TODO: Extract these to uno extensions
// See https://github.com/unoplatform/uno.extensions/discussions/420
public class AppTheme : IAppTheme
{
	private readonly Window _window;
	public AppTheme(Window window)
	{
		_window = window;
	}
	public bool IsDark => SystemThemeHelper.IsRootInDarkMode(_window.Content.XamlRoot);

	public void SetTheme(bool darkMode)
	{
		SystemThemeHelper.SetRootTheme(_window.Content.XamlRoot, darkMode);
	}
}

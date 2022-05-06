using Uno.Toolkit.UI;
using Uno.UI;
using Windows.UI;

namespace ToDo.Helpers;

public static class ThemeService
{
	public static async Task ReapplyCurrentTheme(XamlRoot root)
	{
#if HAS_UNO
		ApplicationHelper.ReapplyApplicationTheme();
		await Task.Yield();
#else
		var isDarkMode = SystemThemeHelper.IsRootInDarkMode(root);
		SystemThemeHelper.SetRootTheme(root, !isDarkMode);
		await Task.Yield();
		SystemThemeHelper.SetRootTheme(root, isDarkMode);
#endif
	}

	public static bool ApplyThemeResources(XamlRoot root, IDictionary<object, object> replacements, string? forTheme = default)
	{
		forTheme ??= SystemThemeHelper.GetRootTheme(root).ToString();

		var appResources = Application.Current.Resources;

		var remainsToBeReplaced = new List<string>(replacements.Keys.Select(x => x.ToString() ?? "null"));

		var changed = ReplaceThemeDictionary(appResources, replacements, forTheme, remainsToBeReplaced);

		if (remainsToBeReplaced.Any())
		{
			Console.Error.WriteLine("These resources were not found: " + string.Join(", ", remainsToBeReplaced));
		}

		return changed;
	}

	private static bool ReplaceThemeDictionary(ResourceDictionary resourcesDictionary, IDictionary<object, object> replacements, string forTheme, IList<string> remainsToBeReplaced)
	{
		var changed = false;

		foreach (var (key, replacement) in replacements)
		{
			// Do replacement for Color
			if (replacement is Color colorToUseAsReplacement && key is string k)
			{
				if (resourcesDictionary.TryGetValue(key, out var valueInThemeDictionary))
				{
					if (valueInThemeDictionary is Color)
					{
						resourcesDictionary[key] = colorToUseAsReplacement;
						remainsToBeReplaced.Remove(k);
						changed = true;
					}
					else
					{
						Console.Error.WriteLine($"Resource {key} is a {valueInThemeDictionary.GetType()}, expected to be a Color");
					}
				}
			}
		}

		if (resourcesDictionary.ThemeDictionaries is { } themeDictionaries
			&& themeDictionaries.TryGetValue(forTheme, out var x)
			&& x is ResourceDictionary themeDictionary)
		{
			ReplaceThemeDictionary(themeDictionary, replacements, forTheme, remainsToBeReplaced);
		}

		if (resourcesDictionary.MergedDictionaries is { Count: > 0 } mergedDictionaries)
		{
			foreach (var mergedDictionary in mergedDictionaries)
			{
				changed = changed || ReplaceThemeDictionary(mergedDictionary, replacements, forTheme, remainsToBeReplaced);
			}
		}

		return changed;
	}
}

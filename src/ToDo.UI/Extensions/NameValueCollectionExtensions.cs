using System.Collections.Specialized;

namespace ToDo.Extensions
{
    public static class NameValueCollectionExtensions
    {
		public static bool TryGetValue(this NameValueCollection nameValueCollection, string key, out string? output)
		{
			var value = nameValueCollection[key];
			if (value == null)
			{
				output = string.Empty;
				return false; 
			}

			output = value;
			return true;
		}
	}
}

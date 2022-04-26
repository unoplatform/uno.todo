using System;
using System.Collections.Generic;
using System.Text;

namespace ToDo.Converters
{
	public class TaskListIconConverter : IValueConverter
	{
		public object? ImportantValue { get; set; }
		public object? TasksValue { get; set; }
		public object? DefaultValue { get; set; }

		public object? Convert(object value, Type targetType, object parameter, string language)
		{
			return (value as TaskList)?.WellknownListName switch
			{
				TaskList.WellknownListNames.Important => ImportantValue,
				TaskList.WellknownListNames.Tasks => TasksValue,

				_ => DefaultValue,
			};
		}

		public object? ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotSupportedException("Only one-way conversion is supported.");
	}
}

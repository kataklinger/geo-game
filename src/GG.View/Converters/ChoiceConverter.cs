using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace GG.View.Converters
{
	public class ChoiceConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Enum)
				return string.Join(" ", Regex.Split(value.ToString(), @"(?<=[a-z])(?=[A-Z])"));

			return value.ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}

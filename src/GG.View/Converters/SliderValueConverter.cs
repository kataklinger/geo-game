using System;
using System.Globalization;
using System.Windows.Data;

namespace GG.View.Converters
{
	public class SliderValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (int)Math.Round((double)value);
		}
	}
}

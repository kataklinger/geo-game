using System;
using System.Globalization;
using System.Windows.Data;

namespace GG.View.Converters
{
	public class PosValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((double)value) + 60;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}

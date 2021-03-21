using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace GG.View.Converters
{
	public class CorrectAnswerColorConverter : IValueConverter
	{
		private readonly Brush CorrectBrush = new SolidColorBrush(Colors.Green);
		private readonly Brush IncorrectBrush = new SolidColorBrush(Colors.Red);
		private readonly Brush DefaultBrush = new SolidColorBrush(Colors.Transparent);

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool)
				return (bool)value ? CorrectBrush : IncorrectBrush;

			return DefaultBrush;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { return null; }
	}
}

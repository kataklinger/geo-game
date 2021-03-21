using System;
using System.Globalization;
using System.Windows.Data;

namespace GG.View.Converters
{
	public class CorrectAnswerImageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool)
				return new Uri((bool)value ? "/Assets/CorrectAnswer.png" : "/Assets/IncorrectAnswer.png", UriKind.Relative);

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { return null; }
	}
}

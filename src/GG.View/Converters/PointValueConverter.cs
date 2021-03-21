using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace GG.View.Converters
{
	public class PointValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var src = value as IList<Tuple<double, double>>;
			if (src != null && targetType == typeof(PointCollection))
			{
				var res = new PointCollection();
				foreach (var p in src)
					res.Add(new Point(p.Item1 + 60, p.Item2 + 60));

				return res;
			}

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { return null; }
	}
}

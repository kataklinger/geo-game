using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GG.Model.Contracts.Game;
using GG.ModelView;

namespace GG.View.Converters
{
	public class FlagBrushConverter : IValueConverter
	{
		private Uri _cachedUrl;
		private WriteableBitmap _cachedImage;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var fill = value as CountryAreaFill;
			if (fill != null)
			{
				if (fill.Selected)
					return new SolidColorBrush(Color.FromArgb((byte)(fill.MarkerOnly ? 85 : 255), 0, 128, 0));

				if (!fill.MarkerOnly)
				{
					if (fill.State == QuestionState.Correct)
					{
						if (_cachedImage == null || fill.Flag != _cachedUrl)
							LoadImage(fill.Flag);

						var scaleX = _cachedImage.PixelWidth / fill.CountryWidth;
						var scaleY = _cachedImage.PixelHeight / fill.CountryHeight;

						var x = scaleX < scaleY;

						double scale = x ? scaleX : scaleY;

						var scaledWidth = (int)(fill.Width * scale);
						var scaledHeight = (int)(fill.Height * scale);

						var scaledX = (int)(fill.X * scale + (_cachedImage.PixelWidth - fill.CountryWidth * scale) / 2);
						var scaledY = (int)(fill.Y * scale + (_cachedImage.PixelHeight - fill.CountryHeight * scale) / 2);

						var brush = new ImageBrush();
						brush.ImageSource = _cachedImage.Crop(scaledX, scaledY, scaledWidth, scaledHeight);
						return brush;
					}
					else if (fill.State == QuestionState.Incorrect)
						return new SolidColorBrush(Colors.Gray);
				}
			}

			return new SolidColorBrush(fill.MarkerOnly ? Color.FromArgb(85, 0, 0, 0) : Color.FromArgb(255, 31, 31, 31));
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { return null; }

		private void LoadImage(Uri image)
		{
			var resource = Application.GetResourceStream(image);
			if (resource != null)
			{
				using (var stream = resource.Stream)
				{
					var source = new BitmapImage();
					source.SetSource(stream);

					_cachedUrl = image;
					_cachedImage = new WriteableBitmap(source);
				}
			}
			else
			{
				_cachedUrl = null;
				_cachedImage = null;
			}
		}
	}
}

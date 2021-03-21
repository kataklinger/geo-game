using System;
using System.Windows.Media.Imaging;
using GG.Model.Contracts.Infrastructure;

namespace GG.App
{
	class ImageDataProvider : IImageDataProvider
	{
		private readonly IAppSettings _settings;
		private readonly IResourceDataProvider _resourceDataProvider;

		public ImageDataProvider(IAppSettings settings, IResourceDataProvider resourceDataProvider)
		{
			_settings = settings;
			_resourceDataProvider = resourceDataProvider;

			Ratio = _settings.IsLowMemory ? 2 : 1;
		}

		public object GetImage(Uri imageLocation)
		{
			using (var stream = _resourceDataProvider.GetDataStream(imageLocation))
			{
				if (stream == null)
					return null;

				var img = new BitmapImage();
				img.SetSource(stream);

				return img;
			}
		}

		public ImageSize GetImageSize(Uri imageLocation)
		{
			using (var stream = _resourceDataProvider.GetDataStream(imageLocation))
			{
				if (stream == null)
					return null;

				var img = new BitmapImage();
				img.SetSource(stream);

				return new ImageSize(img.PixelWidth / Ratio, img.PixelHeight / Ratio);
			}
		}

		private int Ratio { get; set; }
	}
}

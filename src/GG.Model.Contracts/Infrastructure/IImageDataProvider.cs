using System;

namespace GG.Model.Contracts.Infrastructure
{
	public class ImageSize
	{
		public ImageSize() { }
		public ImageSize(int width, int height)
		{
			Width = width;
			Height = height;
		}

		public int Width { get; set; }
		public int Height { get; set; }
	}

	public interface IImageDataProvider
	{
		object GetImage(Uri imageLocation);

		ImageSize GetImageSize(Uri imageLocation);
	}
}

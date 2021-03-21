using System;
using System.IO;
using System.Windows;
using GG.Model.Contracts.Infrastructure;

namespace GG.App
{
	class ResourceDataProvider : IResourceDataProvider
	{
		public Stream GetDataStream(string relativeResourceUrl)
		{
			return GetDataStream(new Uri(relativeResourceUrl, UriKind.Relative));
		}

		public Stream GetDataStream(Uri resourceUri)
		{
			var info = Application.GetResourceStream(resourceUri);
			return info == null ? null : info.Stream;
		}

	}
}

using System;
using System.IO;

namespace GG.Model.Contracts.Infrastructure
{
	public interface IResourceDataProvider
	{
		Stream GetDataStream(string relativeResourceUrl);
		Stream GetDataStream(Uri resourceUri);
	}
}

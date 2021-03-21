using System.Collections.Generic;
using GG.Model.Contracts.Game.Options;
using GG.Model.Contracts.GeoData;

namespace GG.Model.Contracts.Game
{
	public interface ICountrySelector
	{
		string Name { get; }

		IOptions CreateOptions();

		IList<ICountryInfo> Generate(IOptions options);
	}
}

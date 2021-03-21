using System;
using System.Collections.Generic;

namespace GG.Model.Contracts.GeoData
{
	public interface ICountryInfo
	{
		Continent Continent { get; }
		string AdministrativeName { get; }

		IList<GeoPolygon> GeomentryData { get; }
		GeoBounds Bounds { get; }

		Uri LargeFlag { get; }
		Uri SmallFlag { get; }

		IList<IBorderInfo> Borders { get; }

		int ZIndex { get; }

		bool IsSmall { get; }
	}
}

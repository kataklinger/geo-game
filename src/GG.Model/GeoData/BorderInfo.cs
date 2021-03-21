using GG.Model.Contracts.GeoData;

namespace GG.Model.GeoData
{
	class BorderInfo : IBorderInfo
	{
		public ICountryInfo Neighbor { get; private set; }

		public bool HasLandBorder { get; private set; }
		public bool HasMaritimeBorder { get; private set; }
		public bool Excluded { get; private set; }

		public BorderInfo(ICountryInfo neighbor, bool hasLandBorder, bool hasMaritimeBorder, bool excluded)
		{
			Neighbor = neighbor;

			HasLandBorder = hasLandBorder;
			HasMaritimeBorder = hasMaritimeBorder;
			Excluded = excluded;
		}
	}

}

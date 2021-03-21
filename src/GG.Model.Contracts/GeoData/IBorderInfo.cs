
namespace GG.Model.Contracts.GeoData
{
	public interface IBorderInfo
	{
		ICountryInfo Neighbor { get; }

		bool HasLandBorder { get; }
		bool HasMaritimeBorder { get; }
		bool Excluded { get; }
	}
}

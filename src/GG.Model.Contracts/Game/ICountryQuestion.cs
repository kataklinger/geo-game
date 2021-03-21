using GG.Model.Contracts.GeoData;

namespace GG.Model.Contracts.Game
{
	public interface ICountryQuestion : IQuestion
	{
		ICountryInfo Country { get; }

		bool ShowName { get; }
		bool ShowFlag { get; }
	}
}

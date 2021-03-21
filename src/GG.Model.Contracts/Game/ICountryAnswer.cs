using GG.Model.Contracts.GeoData;

namespace GG.Model.Contracts.Game
{
	public interface ICountryAnswer : IAnswer
	{
		ICountryInfo Country { get; }

		bool ShowName { get; }
		bool ShowFlag { get; }
	}
}

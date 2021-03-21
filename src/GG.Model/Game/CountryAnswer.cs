using GG.Model.Contracts.Game;
using GG.Model.Contracts.GeoData;

namespace GG.Model.Game
{
	class CountryAnswer : ICountryAnswer
	{
		public CountryAnswer(ICountryInfo country, bool showName, bool showFlag)
		{
			Country = country;

			ShowName = showName;
			ShowFlag = showFlag;
		}

		public ICountryInfo Country { get; private set; }

		public bool ShowName { get; private set; }
		public bool ShowFlag { get; private set; }
	}
}

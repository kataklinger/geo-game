using System.Collections.Generic;
using GG.Model.Contracts.Game;
using GG.Model.Contracts.GeoData;
using GG.Model.Contracts.Infrastructure;

namespace GG.Model.Game
{
	public class GameProvider : IGameProvider
	{
		private readonly List<IGameDescription> _games;

		public GameProvider(IAppSettings settings, ITimerService timerService, ICountryCollection countryCollection)
		{
			_games = new List<IGameDescription>()
			{
				new FlagsMapGame.GameDescription(settings, timerService, countryCollection),
				new FlagsQuizGame.GameDescription(FlagsQuizGame.Direction.GuessFlag, settings, timerService, countryCollection),
				new FlagsQuizGame.GameDescription(FlagsQuizGame.Direction.GuessName, settings, timerService, countryCollection),
			};
		}

		public IList<IGameDescription> Games { get { return _games; } }
	}
}

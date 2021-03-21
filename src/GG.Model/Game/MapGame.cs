using GG.Model.Contracts.Game;
using GG.Model.Contracts.Game.Options;
using GG.Model.Contracts.Infrastructure;

namespace GG.Model.Game
{
	abstract class MapGame : GameBase, IMapGame
	{
		public MapGame(IOptions gameOptions, ICountrySelector selector, IOptions selectorOptions, ITimerService timerService)
			: base(gameOptions, selector, selectorOptions, timerService) { }
	}
}

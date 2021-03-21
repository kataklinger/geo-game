using GG.Model.Contracts.Game;
using GG.Model.Contracts.Game.Options;
using GG.Model.Contracts.Infrastructure;

namespace GG.Model.Game
{
	abstract class QuizGame : GameBase, IQuizGame
	{
		public QuizGame(IOptions gameOptions, ICountrySelector selector, IOptions selectorOptions, ITimerService timerService)
			: base(gameOptions, selector, selectorOptions, timerService) { }
	}
}

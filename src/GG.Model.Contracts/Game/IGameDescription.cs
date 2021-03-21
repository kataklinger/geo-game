using System.Collections.Generic;
using GG.Model.Contracts.Game.Options;

namespace GG.Model.Contracts.Game
{
	public interface IGameDescription
	{
		string DisplayName { get; }

		IList<ICountrySelector> Selectors { get; }

		IOptions CreateGameOptions();
		IGame CreateGame(IOptions gameOptions, ICountrySelector selector, IOptions selectorOptions);
	}
}

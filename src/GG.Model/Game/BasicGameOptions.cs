using System;
using System.Linq;
using GG.Model.Contracts.Game;
using GG.Model.Game.Options;

namespace GG.Model.Game
{
	class BasicGameOptions : BaseOptions
	{
		public BasicGameOptions()
		{
			_optionsList.AddOption(new ChoiceOption<Difficulty>("difficulty", "Difficulty", Enum
				.GetValues(typeof(Difficulty))
				.Cast<Difficulty>(), Difficulty.Normal));
		}

		public Difficulty Difficulty { get { return _optionsList.GetValue<Difficulty>("difficulty"); } }
	}
}

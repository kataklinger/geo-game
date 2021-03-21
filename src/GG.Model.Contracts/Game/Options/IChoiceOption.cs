using System.Collections.Generic;

namespace GG.Model.Contracts.Game.Options
{
	public interface IChoiceOption : IOption
	{
		IList<object> Choices { get; }
	}
}

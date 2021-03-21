using System.Collections.Generic;

namespace GG.Model.Contracts.Game
{
	public interface IGameProvider
	{
		IList<IGameDescription> Games { get; }
	}
}

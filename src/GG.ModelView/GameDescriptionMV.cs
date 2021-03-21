using GalaSoft.MvvmLight;
using GG.Model.Contracts.Game;

namespace GG.ModelView
{
	public class GameDescriptionMV : ViewModelBase
	{
		public GameDescriptionMV(IGameDescription gameDescription)
		{
			GameDescription = gameDescription;
		}

		public IGameDescription GameDescription { get; private set; }

		public string Name { get { return GameDescription.DisplayName; } }
	}
}

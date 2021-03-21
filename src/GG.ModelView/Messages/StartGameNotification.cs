using GG.Model.Contracts.Game;

namespace GG.ModelView.Messages
{
	public class StartGameNotification
	{
		public StartGameNotification(IGame game)
		{
			Game = game;
		}

		public IGame Game { get; private set; }
	}
}

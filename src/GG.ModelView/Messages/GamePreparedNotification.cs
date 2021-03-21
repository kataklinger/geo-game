using GG.Model.Contracts.Game;

namespace GG.ModelView.Messages
{
	public class GamePreparedNotification
	{
		public GamePreparedNotification(IGame game)
		{
			Game = game;
		}

		public IGame Game { get; set; }
	}
}

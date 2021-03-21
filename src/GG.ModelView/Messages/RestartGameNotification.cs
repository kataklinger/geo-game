
namespace GG.ModelView.Messages
{
	public class RestartGameNotification
	{
		public RestartGameNotification(bool regenerate)
		{
			Regenerate = regenerate;
		}

		public bool Regenerate { get; private set; }
	}
}

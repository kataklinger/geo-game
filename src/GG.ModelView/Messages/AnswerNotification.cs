
namespace GG.ModelView.Messages
{
	public class AnswerNotification
	{
		public AnswerNotification(bool isCorrect)
		{
			IsCorrect = isCorrect;
		}

		public bool IsCorrect { get; private set; }
	}
}

using GG.Model.Contracts.Game;

namespace GG.ModelView.Messages
{
	public class CountryGuessNotification
	{
		public CountryGuessNotification(ICountryQuestion question, ICountryAnswer answer)
		{
			Question = question;
			Answer = answer;
		}

		public ICountryQuestion Question { get; private set; }
		public ICountryAnswer Answer { get; private set; }
	}
}

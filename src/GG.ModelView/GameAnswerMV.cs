using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GG.Model.Contracts.Game;
using GG.Model.Contracts.Infrastructure;
using GG.ModelView.Messages;

namespace GG.ModelView
{
	public class GameAnswerMV : ViewModelBase
	{
		private readonly ICountryQuestion _question;
		private readonly ICountryAnswer _answer;

		private readonly object _flag;

		public GameAnswerMV(IImageDataProvider imageDataProvider, ICountryQuestion question, ICountryAnswer answer)
		{
			_question = question;
			_answer = answer;

			_flag = imageDataProvider.GetImage(_answer.Country.SmallFlag);

			GuessCommand = new RelayCommand(() => MessengerInstance.Send(new CountryGuessNotification(_question, _answer)));
		}

		public bool ShowName { get { return _answer.ShowName; } }

		public string Name { get { return _answer.Country.AdministrativeName; } }

		public bool ShowFlag { get { return _answer.ShowFlag; } }

		public object Flag { get { return _flag; } }

		public RelayCommand GuessCommand { get; private set; }
	}
}

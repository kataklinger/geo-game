using System.Collections.Generic;
using System.Linq;
using GG.Model.Contracts.Game;
using GG.Model.Contracts.Infrastructure;
using GG.ModelView.Messages;

namespace GG.ModelView
{
	public class QuizGameMV : GameMV
	{
		private readonly IImageDataProvider _imageDataProvider;

		private object _flag;
		private bool _showFlag = true;

		private string _name;
		private bool _showName = true;

		private IList<QuizGameAnswerMV> _answers;

		private IEnumerator<ICountryQuestion> _currentQuestion;

		public QuizGameMV(IAppSettings settings, IImageDataProvider imageDataProvider)
			: base(settings)
		{
			_imageDataProvider = imageDataProvider;

			MessengerInstance.Register<CountryGuessNotification>(this, n =>
			{
				if (Game != null)
					Game.AnswerQuestion(n.Question, n.Answer);
			});
		}

		public object Flag
		{
			get { return _flag; }
			private set
			{
				if (_flag != value)
				{
					_flag = value;

					RaisePropertyChanged();
				}
			}
		}

		public bool ShowFlag
		{
			get { return _showFlag; }
			private set
			{
				if (_showFlag != value)
				{
					_showFlag = value;

					RaisePropertyChanged();
				}
			}
		}

		public string Name
		{
			get { return _name; }
			private set
			{
				if (_name != value)
				{
					_name = value;

					RaisePropertyChanged();
				}
			}
		}

		public bool ShowName
		{
			get { return _showName; }
			private set
			{
				if (_showName != value)
				{
					_showName = value;

					RaisePropertyChanged();
				}
			}
		}

		public IList<QuizGameAnswerMV> Answers
		{
			get { return _answers; }
			protected set
			{
				if (_answers != value)
				{
					_answers = value;

					RaisePropertyChanged();
				}
			}
		}

		protected override bool CanHandleGame(IGame game)
		{
			return game is IQuizGame;
		}

		protected override void HandleNewGame(bool freshStart)
		{
			if (freshStart)
				Game.OnQuestionStateChanged += OnQuestionStateChanged;

			_currentQuestion = Game.Questions
			   .Cast<ICountryQuestion>()
			   .GetEnumerator();

			NextQuestion();
		}

		protected override void HandleEndGame()
		{
			Game.OnQuestionStateChanged -= OnQuestionStateChanged;

			_currentQuestion = null;
		}

		protected override IList<EndGameDetailsMV> AnswersDetails
		{
			get
			{
				return Game.Questions
					.Cast<ICountryQuestion>()
					.OrderBy(o => o.AnswerOrder)
					.Select(o =>
					{
						var answer = o.Answer as ICountryAnswer;

						return new EndGameDetailsMV(o.Country.AdministrativeName, o.Country.SmallFlag,
							answer != null ? answer.Country.AdministrativeName : string.Empty,
							answer != null ? answer.Country.SmallFlag : null,
							o.State == QuestionState.Correct);
					})
					.ToList();
			}
		}

		private void NextQuestion()
		{
			if (_currentQuestion != null && _currentQuestion.MoveNext())
			{
				var question = _currentQuestion.Current;
				Game.AskQuestion(question);

				Flag = _imageDataProvider.GetImage(question.Country.SmallFlag);
				ShowFlag = question.ShowFlag;

				Name = question.Country.AdministrativeName;
				ShowName = question.ShowName;

				Answers = Game.Answers
					.Select((o, i) => new QuizGameAnswerMV(_imageDataProvider, question, (ICountryAnswer)o, i % 2, i / 2))
					.ToList();
			}
		}

		private void OnQuestionStateChanged(object sender, IQuestion e)
		{
			NextQuestion();
		}
	}
}

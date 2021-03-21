using System;
using GG.Model.Contracts.Game;
using GG.Model.Contracts.GeoData;

namespace GG.Model.Game
{
	class CountryQuestion : ICountryQuestion
	{
		private bool _setOnIncorrect;

		private IAnswer _answer;
		private QuestionState _state;

		public CountryQuestion(bool setOnIncorrect, ICountryInfo country, bool showName, bool showFlag)
		{
			_setOnIncorrect = setOnIncorrect;

			State = QuestionState.Unanswered;
			Country = country;

			ShowName = showName;
			ShowFlag = showFlag;
		}

		public event EventHandler<IQuestion> OnStateChanged;

		public IAnswer Answer
		{
			get { return _answer; }
			set
			{
				if (State == QuestionState.Unanswered)
				{
					bool correct = value != null && Country == ((ICountryAnswer)value).Country;
					if (correct || _setOnIncorrect)
					{
						_answer = value;
						State = correct ? QuestionState.Correct : QuestionState.Incorrect;
					}
				}
			}
		}

		public QuestionState State
		{
			get { return _state; }
			private set
			{
				if (_state != value)
				{
					_state = value;

					var evt = OnStateChanged;
					if (evt != null)
						evt(this, this);
				}
			}
		}

		public int AnswerOrder { get; set; }

		public ICountryInfo Country { get; private set; }

		public bool ShowName { get; private set; }
		public bool ShowFlag { get; private set; }

		public void Clear()
		{
			AnswerOrder = 0;

			_answer = null;
			State = QuestionState.Unanswered;

		}

	}
}

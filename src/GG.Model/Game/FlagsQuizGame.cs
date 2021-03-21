using System;
using System.Collections.Generic;
using System.Linq;
using GG.Model.Contracts.Game;
using GG.Model.Contracts.Game.Options;
using GG.Model.Contracts.GeoData;
using GG.Model.Contracts.Infrastructure;
using GG.Model.Game.Selection;

namespace GG.Model.Game
{
	class FlagsQuizGame : QuizGame
	{
		public enum Direction
		{
			GuessFlag,
			GuessName
		}

		public class GameDescription : IGameDescription
		{
			private readonly Direction _direction;

			private readonly ITimerService _timerService;
			private readonly ICountryCollection _countryCollection;

			public GameDescription(Direction direction, IAppSettings settings, ITimerService timerService, ICountryCollection countryCollection)
			{
				_direction = direction;

				_timerService = timerService;
				_countryCollection = countryCollection;

				Selectors = new List<ICountrySelector>
				{
					new ContinentSelector(settings, countryCollection),
					new ContinentFragmentSelector(settings, countryCollection),
					new RegionSelector(settings, countryCollection),
					new NeighborsSelector(settings, countryCollection),
				};
			}

			public string DisplayName { get { return _direction == Direction.GuessFlag ? "Flags Quiz" : "Name Quiz"; } }

			public IList<ICountrySelector> Selectors { get; private set; }

			public IOptions CreateGameOptions() { return new BasicGameOptions(); }
			public IGame CreateGame(IOptions gameOptions, ICountrySelector selector, IOptions selectorOptions)
			{
				return new FlagsQuizGame(_direction, gameOptions, selector, selectorOptions, _timerService, _countryCollection);
			}
		}

		private static int[] _timings = new int[] { 10, 7, 5 };

		private readonly IList<ICountryInfo> _randomizedContryList;

		private readonly Direction _direction;

		private IQuestion _currentQuestion = null;

		public FlagsQuizGame(Direction direction, IOptions gameOptions, ICountrySelector selector, IOptions selectorOptions, ITimerService timerService, ICountryCollection countryCollection)
			: base(gameOptions, selector, selectorOptions, timerService)
		{
			_direction = direction;

			_randomizedContryList = countryCollection.Countries
				.Select(c => new { Country = c, Order = _random.Next() })
				.OrderBy(i => i.Order)
				.Select(i => i.Country)
				.ToList();
		}

		public override Difficulty Difficulty { get { return ((BasicGameOptions)GameOptions).Difficulty; } }

		public override int HitScore { get { return (int)Difficulty + 1; } }

		public override void AskQuestion(IQuestion question)
		{
			var isHard = Difficulty == Difficulty.Hard;
			var isNormal = Difficulty == Difficulty.Normal;

			var country = ((ICountryQuestion)question).Country;

			IEnumerable<ICountryInfo> source = _randomizedContryList
				.Where(c => c != country && c.Continent == country.Continent)
				.ToList();

			if (source.Count() < 6)
			{
				source = source
					.Union(_randomizedContryList
						.Where(c => c != country))
					.ToList();
			}

			source = source
				.Skip(_random.Next() % (source.Count() - 6))
				.ToList();

			var count = 3;
			if (isNormal || isHard)
			{
				source = country.Borders
					.Select(b => b.Neighbor)
					.Select(c => new { Country = c, Order = _random.Next() })
					.OrderBy(i => i.Order)
					.Select(i => i.Country)
					.Union(source)
					.ToList();

				count = 5;
			}

			Answers = source
				.Distinct()
				.Take(count)
				.Union(Enumerable.Repeat(country, 1))
				.Select(c => new { Country = c, Order = _random.Next() })
				.OrderBy(i => i.Order)
				.Select(i => new CountryAnswer(i.Country, _direction == Direction.GuessName, _direction == Direction.GuessFlag) as IAnswer)
				.ToList();

			RestartAnswerTime();

			_currentQuestion = question;
		}

		public override void AnswerQuestion(IQuestion question, IAnswer answer)
		{
			if (question.State == QuestionState.Unanswered)
			{
				var hitScore = HitScore;

				question.Answer = answer;

				CorrectAnswers = Questions.Count(o => o.State == QuestionState.Correct);
				IncorrectAnswers = Questions.Count(o => o.State == QuestionState.Incorrect);

				question.AnswerOrder = CorrectAnswers + IncorrectAnswers;

				if (question.State == QuestionState.Correct)
					TotalScore += hitScore;

				Completed = Questions.All(o => o.State != QuestionState.Unanswered);
			}
		}

		protected override void AnswerTimeExpired()
		{
			if (_currentQuestion != null)
				AnswerQuestion(_currentQuestion, null);
		}

		protected override IQuestion CreateQuestion(ICountryInfo country)
		{
			return new CountryQuestion(true, country, _direction == Direction.GuessFlag, _direction == Direction.GuessName);
		}

		protected override void Restart()
		{
			var diff = (int)Difficulty;

			RestartAnswerTime();

			Completed = false;
			MaxScore = Questions.Count * (diff + 1);

			TotalQuestions = Questions.Count;
			TotalScore = CorrectAnswers = IncorrectAnswers = 0;

			MaxAnswerTime = _timings[diff];
		}
	}
}

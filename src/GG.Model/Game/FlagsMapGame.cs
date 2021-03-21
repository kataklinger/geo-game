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
	class FlagsMapGame : MapGame
	{
		public class GameDescription : IGameDescription
		{
			private readonly ITimerService _timerService;
			private readonly ICountryCollection _countryCollection;

			public GameDescription(IAppSettings settings, ITimerService timerService, ICountryCollection countryCollection)
			{
				_timerService = timerService;
				_countryCollection = countryCollection;

				Selectors = new List<ICountrySelector>
				{
					new ContinentSelector(settings, countryCollection),
					new RegionSelector(settings, countryCollection),
					new NeighborsSelector(settings, countryCollection),
				};
			}

			public string DisplayName { get { return "Flags Map"; } }

			public IList<ICountrySelector> Selectors { get; private set; }

			public IOptions CreateGameOptions() { return new BasicGameOptions(); }
			public IGame CreateGame(IOptions gameOptions, ICountrySelector selector, IOptions selectorOptions)
			{
				return new FlagsMapGame(gameOptions, selector, selectorOptions, _timerService, _countryCollection);
			}
		}

		private static int[] _timings = new int[] { 10, 7, 5 };

		private readonly IList<ICountryInfo> _randomizedContryList;

		private readonly IDictionary<IQuestion, IList<IAnswer>> _answers = new Dictionary<IQuestion, IList<IAnswer>>();

		public FlagsMapGame(IOptions gameOptions, ICountrySelector selector, IOptions selectorOptions, ITimerService timerService, ICountryCollection countryCollection)
			: base(gameOptions, selector, selectorOptions, timerService)
		{
			_randomizedContryList = countryCollection.Countries
				.Select(c => new { Country = c, Order = _random.Next() })
				.OrderBy(i => i.Order)
				.Select(i => i.Country)
				.ToList();
		}

		public override Difficulty Difficulty { get { return ((BasicGameOptions)GameOptions).Difficulty; } }

		public override int HitScore
		{
			get
			{
				var diff = (int)Difficulty + 1;
				return Math.Max(1, diff * Questions.Count / (1 + (AnswerTime / _timings[(int)Difficulty])));
			}
		}

		public override void AskQuestion(IQuestion question)
		{
			IList<IAnswer> answers;
			if (!_answers.TryGetValue(question, out answers))
			{
				answers = GenerateAnswers((ICountryQuestion)question);
				_answers[question] = answers;
			}

			Answers = answers;
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

				RestartAnswerTime();
			}
		}

		protected override void AnswerTimeExpired()
		{
		}

		protected override IQuestion CreateQuestion(ICountryInfo country)
		{
			return new CountryQuestion(true, country, true, true);
		}

		protected override void Restart()
		{
			RestartAnswerTime();

			Completed = false;
			MaxScore = ((int)Difficulty + 1) * Questions.Count * Questions.Count;

			TotalQuestions = Questions.Count;
			TotalScore = CorrectAnswers = IncorrectAnswers = 0;

			MaxAnswerTime = 0;
		}

		private IList<IAnswer> GenerateAnswers(ICountryQuestion question)
		{
			var country = question.Country;

			var isHard = Difficulty == Difficulty.Hard;
			var isNormal = Difficulty == Difficulty.Normal;

			var excluded = new HashSet<ICountryInfo>(Questions
				.Cast<ICountryQuestion>()
				.Where(o => o == question || o.State == QuestionState.Correct || (o.State == QuestionState.Incorrect && !isHard))
				.Select(o => o.Country));

			IEnumerable<ICountryInfo> source = Questions
				.Cast<ICountryQuestion>()
				.Select(o => o.Country)
				.Where(c => !excluded.Contains(c))
				.Select(c => new { Country = c, Order = _random.Next() })
				.OrderBy(i => i.Order)
				.Select(i => i.Country)
				.Union(_randomizedContryList
					.Where(c => !excluded.Contains(c))
					.Skip(_random.Next() % (_randomizedContryList.Count - excluded.Count - 7)))
				.ToList();

			var count = 5;

			if (isNormal || isHard)
			{
				source = country.Borders
					.Where(b => !b.Excluded && !excluded.Contains(b.Neighbor))
					.Select(b => b.Neighbor)
					.Select(c => new { Country = c, Order = _random.Next() })
					.OrderBy(i => i.Order)
					.Select(i => i.Country)
					.Union(source)
					.ToList();

				count = 7;
			}

			return source
				.Distinct()
				.Take(count)
				.Union(Enumerable.Repeat(country, 1))
				.OrderBy(c => c.AdministrativeName)
				.Select(c => new CountryAnswer(c, !isHard, true) as IAnswer)
				.ToList();
		}
	}
}

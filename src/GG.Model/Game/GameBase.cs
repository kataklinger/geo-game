using System;
using System.Collections.Generic;
using System.Linq;
using GG.Model.Contracts.Game;
using GG.Model.Contracts.Game.Options;
using GG.Model.Contracts.GeoData;
using GG.Model.Contracts.Infrastructure;

namespace GG.Model.Game
{
	abstract class GameBase : IGame
	{
		protected static readonly Random _random = new Random();

		private readonly ITimerService _timerService;

		private readonly ICountrySelector _selector;
		private readonly IOptions _selectorOptions;

		private IList<IQuestion> _questions;
		private IList<IAnswer> _answers;

		private int _totalScore;
		private int _maxScore;

		private int _correctAnswers;
		private int _incorrectAnswers;
		private int _totalQuestions;

		private int _maxAnswerTime;

		private bool _completed;

		private bool _paused = true;

		private DateTime _answerTimestamp;
		private DateTime _pauseTimestamp;
		private DateTime _startTimestamp;

		public GameBase(IOptions gameOptions, ICountrySelector selector, IOptions selectorOptions, ITimerService timerService)
		{
			_timerService = timerService;
			_timerService.Subscribe(RaiseHitScoreChanged);

			GameOptions = gameOptions;

			_selector = selector;
			_selectorOptions = selectorOptions;
		}

		public virtual void Dispose()
		{
			_timerService.Unsubscribe(RaiseHitScoreChanged);
		}

		public event EventHandler<IList<IQuestion>> OnQuestionsChanged;
		public event EventHandler<IList<IAnswer>> OnAnswersChanged;

		public event EventHandler<IQuestion> OnQuestionStateChanged;

		public event EventHandler<int> OnHitScoreChanged;
		public event EventHandler<int> OnTotalScoreChanged;
		public event EventHandler<int> OnMaxScoreChanged;

		public event EventHandler<int> OnCorrectAnswersChanged;
		public event EventHandler<int> OnIncorrectAnswersChanged;
		public event EventHandler<int> OnTotalAnswersChanged;
		public event EventHandler<int> OnTotalQuestionsChanged;

		public event EventHandler<int> OnAnswerTimeChanged;
		public event EventHandler<int> OnMaxAnswerTimeChanged;

		public event EventHandler<bool> OnCompletedChanged;

		public abstract Difficulty Difficulty { get; }

		public IList<IQuestion> Questions
		{
			get { return _questions; }
			private set
			{
				if (_questions != value)
				{
					_questions = value;

					var evt = OnQuestionsChanged;
					if (evt != null)
						evt(this, _questions);
				}
			}
		}

		public IList<IAnswer> Answers
		{
			get { return _answers; }
			protected set
			{
				if (_answers != value)
				{
					_answers = value;

					var evt = OnAnswersChanged;
					if (evt != null)
						evt(this, _answers);
				}
			}
		}

		public abstract int HitScore { get; }

		public int TotalScore
		{
			get { return _totalScore; }
			protected set
			{
				if (_totalScore != value)
				{
					_totalScore = value;

					var evt = OnTotalScoreChanged;
					if (evt != null)
						evt(this, _totalScore);
				}
			}
		}

		public int MaxScore
		{
			get { return _maxScore; }
			protected set
			{
				if (_maxScore != value)
				{
					_maxScore = value;

					var evt = OnMaxScoreChanged;
					if (evt != null)
						evt(this, _maxScore);
				}
			}
		}

		public int CorrectAnswers
		{
			get { return _correctAnswers; }
			protected set
			{
				if (_correctAnswers != value)
				{
					_correctAnswers = value;

					var evt1 = OnCorrectAnswersChanged;
					if (evt1 != null)
						evt1(this, _correctAnswers);

					var evt2 = OnTotalAnswersChanged;
					if (evt2 != null)
						evt2(this, _correctAnswers + _incorrectAnswers);
				}
			}
		}

		public int IncorrectAnswers
		{
			get { return _incorrectAnswers; }
			protected set
			{
				if (_incorrectAnswers != value)
				{
					_incorrectAnswers = value;

					var evt1 = OnIncorrectAnswersChanged;
					if (evt1 != null)
						evt1(this, _incorrectAnswers);

					var evt2 = OnTotalAnswersChanged;
					if (evt2 != null)
						evt2(this, _correctAnswers + _incorrectAnswers);
				}
			}
		}

		public int TotalAnswers { get { return _correctAnswers + _incorrectAnswers; } }

		public int TotalQuestions
		{
			get { return _totalQuestions; }
			protected set
			{
				if (_totalQuestions != value)
				{
					_totalQuestions = value;

					var evt = OnTotalQuestionsChanged;
					if (evt != null)
						evt(this, _totalQuestions);
				}
			}
		}

		public int AnswerTime { get { return (int)(DateTime.Now - _answerTimestamp).TotalSeconds; } }

		public virtual int MaxAnswerTime
		{
			get { return _maxAnswerTime; }
			protected set
			{
				if (_maxAnswerTime != value)
				{
					_maxAnswerTime = value;

					var evt = OnMaxAnswerTimeChanged;
					if (evt != null)
						evt(this, _maxAnswerTime);
				}
			}
		}

		public int TotalTime { get { return (int)(DateTime.Now - _startTimestamp).TotalSeconds; } }

		public bool Completed
		{
			get { return _completed; }
			protected set
			{
				if (_completed != value)
				{
					_completed = value;

					var evt = OnCompletedChanged;
					if (evt != null)
						evt(this, _completed);
				}
			}
		}

		public void Restart(bool regenerate)
		{
			if (regenerate)
			{
				if (Questions != null)
				{
					foreach (var q in Questions)
						q.OnStateChanged -= ForwardQuestionStateChanged;
				}

				Questions = _selector.Generate(_selectorOptions)
					.Select(CreateQuestion)
					.ToList();

				foreach (var q in Questions)
					q.OnStateChanged += ForwardQuestionStateChanged;

				if (!Questions.Any())
					throw new GameGenerationException();
			}
			else
			{
				foreach (var q in Questions)
					q.Clear();
			}

			Restart();

			_pauseTimestamp = _startTimestamp = DateTime.Now;
			_paused = true;
		}

		public void Pause()
		{
			if (!_paused)
			{
				_pauseTimestamp = DateTime.Now;
				_paused = true;
			}
		}

		public void Resume()
		{
			if (_paused)
			{
				var pauseTime = DateTime.Now - _pauseTimestamp;
				_answerTimestamp += pauseTime;
				_startTimestamp += pauseTime;

				_paused = false;
			}
		}

		protected IOptions GameOptions { get; private set; }

		public abstract void AskQuestion(IQuestion question);
		public abstract void AnswerQuestion(IQuestion question, IAnswer answer);

		protected abstract void AnswerTimeExpired();

		protected abstract void Restart();

		protected abstract IQuestion CreateQuestion(ICountryInfo country);

		protected void RestartAnswerTime()
		{
			_answerTimestamp = DateTime.Now;
		}

		private void ForwardQuestionStateChanged(object sender, IQuestion e)
		{
			var evt = OnQuestionStateChanged;
			if (evt != null)
				OnQuestionStateChanged(this, e);
		}

		private void RaiseHitScoreChanged(object sender, EventArgs e)
		{
			if (!_paused)
			{
				var time = AnswerTime;
				var evt1 = OnAnswerTimeChanged;
				if (evt1 != null)
					evt1(this, time);

				var evt2 = OnHitScoreChanged;
				if (evt2 != null)
					evt2(this, HitScore);

				if (MaxAnswerTime > 0 && time >= MaxAnswerTime)
					AnswerTimeExpired();
			}
		}
	}
}

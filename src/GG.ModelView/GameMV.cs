using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GG.Model.Contracts.Game;
using GG.Model.Contracts.Infrastructure;
using GG.ModelView.Messages;

namespace GG.ModelView
{
	public abstract class GameMV : PageBaseMV
	{
		private static readonly Random _random = new Random();

		private bool _isAnswerCorrect;
		private bool _showAnswerCorrectness;
		private int _shownAnswerCount;

		public GameMV(IAppSettings settings)
			: base(settings)
		{
			MessengerInstance.Register<StartGameNotification>(this, n =>
			{
				if (CanHandleGame(n.Game))
				{
					try
					{
						Game = n.Game;

						Game.OnHitScoreChanged += OnHitScoreChanged;
						Game.OnTotalScoreChanged += OnTotalScoreChanged;
						Game.OnMaxScoreChanged += OnMaxScoreChanged;

						Game.OnCorrectAnswersChanged += OnCorrectAnswersChanged;
						Game.OnIncorrectAnswersChanged += OnIncorrectAnswersChanged;
						Game.OnTotalAnswersChanged += OnTotalAnswersChanged;
						Game.OnTotalQuestionsChanged += OnTotalQuestionsChanged;

						Game.OnAnswerTimeChanged += OnAnswerTimeChanged;
						Game.OnMaxAnswerTimeChanged += OnMaxAnswerTimeChanged;

						Game.OnCompletedChanged += OnCompletedChanged;

						Game.OnQuestionStateChanged += OnQuestionStateChanged;

						Game.Restart(true);

						HandleNewGame(true);

						MessengerInstance.Send(new GamePreparedNotification(Game));

						Game.Resume();
					}
					catch (GameGenerationException)
					{
						MessengerInstance.Send(new GameGenerationFailedNotification());
					}
				}
			});

			MessengerInstance.Register<StopGameNotification>(this, n => Clean());
			MessengerInstance.Register<EndGameNotification>(this, n => Clean());

			MessengerInstance.Register<ResumeGameNotification>(this, n =>
			{
				if (Game != null)
					Game.Resume();
			});

			MessengerInstance.Register<RestartGameNotification>(this, n =>
			{
				if (Game != null)
				{
					try
					{
						Game.Restart(n.Regenerate);

						HandleNewGame(false);

						Game.Resume();
					}
					catch (GameGenerationException)
					{
						MessengerInstance.Send(new GameGenerationFailedNotification());
					}
				}
			});

			PauseGameCommand = new RelayCommand(() =>
			{
				Game.Pause();

				MessengerInstance.Send(new PauseGameNotification());
			});

			StopGameCommand = new RelayCommand(() => MessengerInstance.Send(new StopGameNotification()));
		}

		public bool IsAnswerCorrect
		{
			get { return _isAnswerCorrect; }
			private set
			{
				if (_isAnswerCorrect != value)
				{
					_isAnswerCorrect = value;

					RaisePropertyChanged();
				}

				IndicateCorrectness();
			}
		}

		public bool ShowAnswerCorrectness
		{
			get { return _showAnswerCorrectness; }
			private set
			{
				if (_showAnswerCorrectness != value)
				{
					_showAnswerCorrectness = value;

					RaisePropertyChanged();
				}
			}
		}

		public int HitScore { get { return Game.HitScore; } }
		public int TotalScore { get { return Game.TotalScore; } }
		public int MaxScore { get { return Game.MaxScore; } }

		public bool Completed { get { return Game.Completed; } }

		public int CorrectAnswers { get { return Game.CorrectAnswers; } }
		public int IncorrectAnswers { get { return Game.IncorrectAnswers; } }
		public int TotalAnswers { get { return Game.TotalAnswers; } }
		public int TotalQuestions { get { return Game.TotalQuestions; } }

		public int AnswerTime { get { return Game.AnswerTime; } }
		public int MaxAnswerTime { get { return Game.MaxAnswerTime; } }

		public RelayCommand CancelGuessCommand { get; protected set; }
		public RelayCommand PauseGameCommand { get; protected set; }
		public RelayCommand StopGameCommand { get; protected set; }

		protected abstract bool CanHandleGame(IGame game);
		protected abstract void HandleNewGame(bool freshStart);
		protected abstract void HandleEndGame();

		protected abstract IList<EndGameDetailsMV> AnswersDetails { get; }

		protected IGame Game { get; private set; }

		private void Clean()
		{
			if (Game != null)
			{
				HandleEndGame();

				Game.OnHitScoreChanged -= OnHitScoreChanged;
				Game.OnTotalScoreChanged -= OnTotalScoreChanged;
				Game.OnMaxScoreChanged -= OnMaxScoreChanged;

				Game.OnCorrectAnswersChanged -= OnCorrectAnswersChanged;
				Game.OnIncorrectAnswersChanged -= OnIncorrectAnswersChanged;
				Game.OnTotalAnswersChanged -= OnTotalAnswersChanged;
				Game.OnTotalQuestionsChanged -= OnTotalQuestionsChanged;

				Game.OnAnswerTimeChanged -= OnAnswerTimeChanged;
				Game.OnMaxAnswerTimeChanged -= OnMaxAnswerTimeChanged;

				Game.OnCompletedChanged -= OnCompletedChanged;

				Game.OnQuestionStateChanged -= OnQuestionStateChanged;

				Game.Dispose();
				Game = null;
			}
		}

		private async void IndicateCorrectness()
		{
			_shownAnswerCount++;
			if (_shownAnswerCount == 1)
				ShowAnswerCorrectness = true;

			await Task.Delay(1500);

			_shownAnswerCount--;
			if (_shownAnswerCount == 0)
				ShowAnswerCorrectness = false;
		}

		private void OnQuestionStateChanged(object sender, IQuestion e)
		{
			if (e.State != QuestionState.Unanswered)
			{
				IsAnswerCorrect = e.State == QuestionState.Correct;

				MessengerInstance.Send(new AnswerNotification(IsAnswerCorrect));
			}
		}

		private void OnHitScoreChanged(object sender, int e) { RaisePropertyChanged("HitScore"); }
		private void OnTotalScoreChanged(object sender, int e) { RaisePropertyChanged("TotalScore"); }
		private void OnMaxScoreChanged(object sender, int e) { RaisePropertyChanged("MaxScore"); }
		private void OnCorrectAnswersChanged(object sender, int e) { RaisePropertyChanged("CorrectAnswers"); }
		private void OnIncorrectAnswersChanged(object sender, int e) { RaisePropertyChanged("IncorrectAnswers"); }
		private void OnTotalAnswersChanged(object sender, int e) { RaisePropertyChanged("TotalAnswers"); }
		private void OnTotalQuestionsChanged(object sender, int e) { RaisePropertyChanged("TotalQuestions"); }
		private void OnAnswerTimeChanged(object sender, int e) { RaisePropertyChanged("AnswerTime"); }
		private void OnMaxAnswerTimeChanged(object sender, int e) { RaisePropertyChanged("MaxAnswerTime"); }
		private void OnCompletedChanged(object sender, bool e)
		{
			RaisePropertyChanged("Completed");

			if (Game != null && Game.Completed)
				MessengerInstance.Send(new EndGameNotification(Game.CorrectAnswers, Game.TotalQuestions, Game.TotalTime, Game.TotalScore, Game.MaxScore, Game.Difficulty, AnswersDetails));
		}
	}
}

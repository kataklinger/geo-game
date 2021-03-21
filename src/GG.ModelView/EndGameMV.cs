using System;
using System.Collections.Generic;
using GG.Model.Contracts.Game;
using GG.Model.Contracts.Infrastructure;
using GG.ModelView.Messages;

namespace GG.ModelView
{
	public class EndGameMV : PageBaseMV
	{
		private int _correctAnswers;
		private int _totalQuestions;

		private int _successRate;

		private int _totalTime;

		private int _totalPoints;
		private int _maxPoints;

		private Difficulty _difficulty;

		private IList<EndGameDetailsMV> _details;

		public EndGameMV(IAppSettings settings)
			: base(settings)
		{
			MessengerInstance.Register<EndGameNotification>(this, n =>
			{
				CorrectAnswers = n.CorrectAnswers;
				TotalQuestions = n.TotalQuestions;

				SuccessRate = (int)Math.Round((100.0 * CorrectAnswers) / TotalQuestions);

				TotalTime = n.TotalTime;

				TotalPoints = n.TotalPoints;
				MaxPoints = n.MaxPoints;

				Difficulty = n.Difficulty;

				Details = n.Details;
			});
		}

		public int CorrectAnswers
		{
			get { return _correctAnswers; }
			private set
			{
				if (_correctAnswers != value)
				{
					_correctAnswers = value;

					RaisePropertyChanged();
				}
			}
		}

		public int TotalQuestions
		{
			get { return _totalQuestions; }
			private set
			{
				if (_totalQuestions != value)
				{
					_totalQuestions = value;

					RaisePropertyChanged();
				}
			}
		}

		public int SuccessRate
		{
			get { return _successRate; }
			private set
			{
				if (_successRate != value)
				{
					_successRate = value;

					RaisePropertyChanged();
				}
			}
		}

		public int TotalTime
		{
			get { return _totalTime; }
			private set
			{
				if (_totalTime != value)
				{
					_totalTime = value;

					RaisePropertyChanged();
				}
			}
		}

		public int TotalPoints
		{
			get { return _totalPoints; }
			private set
			{
				if (_totalPoints != value)
				{
					_totalPoints = value;

					RaisePropertyChanged();
				}
			}
		}

		public int MaxPoints
		{
			get { return _maxPoints; }
			private set
			{
				if (_maxPoints != value)
				{
					_maxPoints = value;

					RaisePropertyChanged();
				}
			}
		}

		public Difficulty Difficulty
		{
			get { return _difficulty; }
			private set
			{
				if (_difficulty != value)
				{
					_difficulty = value;

					RaisePropertyChanged();
				}
			}
		}

		public IList<EndGameDetailsMV> Details
		{
			get { return _details; }
			private set
			{
				if (_details != value)
				{
					_details = value;

					RaisePropertyChanged();
				}
			}
		}
	}
}

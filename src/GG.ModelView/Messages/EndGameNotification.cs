using System.Collections.Generic;
using GG.Model.Contracts.Game;

namespace GG.ModelView.Messages
{
	public class EndGameNotification
	{
		public EndGameNotification(int correctAnswers, int totalQuestions, int totalTime, int totalPoints, int maxPoints,
			Difficulty difficulty, IList<EndGameDetailsMV> details)
		{
			CorrectAnswers = correctAnswers;
			TotalQuestions = totalQuestions;

			TotalTime = totalTime;

			TotalPoints = totalPoints;
			MaxPoints = maxPoints;

			Difficulty = difficulty;

			Details = details;
		}

		public int CorrectAnswers { get; private set; }
		public int TotalQuestions { get; private set; }

		public int TotalTime { get; private set; }

		public int TotalPoints { get; private set; }
		public int MaxPoints { get; private set; }

		public Difficulty Difficulty { get; private set; }

		public IList<EndGameDetailsMV> Details { get; private set; }
	}
}

using System;
using System.Collections.Generic;

namespace GG.Model.Contracts.Game
{
	public interface IGame : IDisposable
	{
		event EventHandler<IList<IQuestion>> OnQuestionsChanged;
		event EventHandler<IList<IAnswer>> OnAnswersChanged;

		event EventHandler<IQuestion> OnQuestionStateChanged;

		event EventHandler<int> OnHitScoreChanged;
		event EventHandler<int> OnTotalScoreChanged;
		event EventHandler<int> OnMaxScoreChanged;

		event EventHandler<int> OnCorrectAnswersChanged;
		event EventHandler<int> OnIncorrectAnswersChanged;
		event EventHandler<int> OnTotalAnswersChanged;
		event EventHandler<int> OnTotalQuestionsChanged;

		event EventHandler<int> OnAnswerTimeChanged;
		event EventHandler<int> OnMaxAnswerTimeChanged;

		event EventHandler<bool> OnCompletedChanged;

		Difficulty Difficulty { get; }

		IList<IQuestion> Questions { get; }
		IList<IAnswer> Answers { get; }

		int HitScore { get; }
		int TotalScore { get; }
		int MaxScore { get; }

		int CorrectAnswers { get; }
		int IncorrectAnswers { get; }
		int TotalAnswers { get; }
		int TotalQuestions { get; }

		int AnswerTime { get; }
		int MaxAnswerTime { get; }

		int TotalTime { get; }

		bool Completed { get; }

		void Restart(bool regenerate);

		void Pause();
		void Resume();

		void AskQuestion(IQuestion question);
		void AnswerQuestion(IQuestion question, IAnswer answer);
	}
}

using System;
using GalaSoft.MvvmLight;

namespace GG.ModelView
{
	public class EndGameDetailsMV : ViewModelBase
	{
		public EndGameDetailsMV(string questionName, Uri questionImage, string answerName, Uri answerImage, bool isCorrect)
		{
			QuestionName = questionName;
			QuestionImage = questionImage;

			AnswerName = answerName;
			AnswerImage = answerImage;

			IsCorrect = isCorrect;
			IsIncorrect = !isCorrect;
		}

		public string QuestionName { get; private set; }
		public Uri QuestionImage { get; private set; }

		public string AnswerName { get; private set; }
		public Uri AnswerImage { get; private set; }

		public bool IsCorrect { get; private set; }
		public bool IsIncorrect { get; private set; }
	}
}

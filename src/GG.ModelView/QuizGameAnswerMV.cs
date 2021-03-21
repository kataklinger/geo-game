using GG.Model.Contracts.Game;
using GG.Model.Contracts.Infrastructure;

namespace GG.ModelView
{
	public class QuizGameAnswerMV : GameAnswerMV
	{
		public QuizGameAnswerMV(IImageDataProvider imageDataProvider, ICountryQuestion question, ICountryAnswer answer, int column, int row)
			: base(imageDataProvider, question, answer)
		{
			Column = column;
			Row = row;
		}

		public int Column { get; private set; }
		public int Row { get; private set; }
	}
}

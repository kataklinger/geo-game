using System;
using GG.Model.Contracts.GeoData;

namespace GG.Model.Contracts.Game
{
	public interface IQuestion
	{
		event EventHandler<IQuestion> OnStateChanged;

		IAnswer Answer { get; set; }
		QuestionState State { get; }

		int AnswerOrder { get; set; }

		void Clear();
	}
}

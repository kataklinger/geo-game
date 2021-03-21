using System;
using GG.Model.Contracts.Game;

namespace GG.ModelView
{
	public class CircleMapAreaMV : MapAreaMV
	{
		public CircleMapAreaMV(SelectionMarker selectionMarker, ICountryQuestion question, double gloablLeft, double gloablTop, double gloablScale)
			: base(selectionMarker, question, question.Country.Bounds, true)
		{
			var bounds = question.Country.Bounds;

			var width = (bounds.Right - bounds.Left) * gloablScale;
			var height = (bounds.Bottom - bounds.Top) * gloablScale;

			Size = Math.Sqrt(Math.Pow(width, 2) + Math.Pow(height, 2)) + 40;

			Left = (bounds.Left - gloablLeft) * gloablScale - (Size - width) / 2;
			Top = (bounds.Top - gloablTop) * gloablScale - (Size - height) / 2;

			question.OnStateChanged += (s, e) => Visible = e.State == QuestionState.Unanswered;
		}

		public double Size { get; private set; }

		public double Left { get; private set; }
		public double Top { get; private set; }
	}
}

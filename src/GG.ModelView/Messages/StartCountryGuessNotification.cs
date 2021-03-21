using GG.Model.Contracts.Game;

namespace GG.ModelView.Messages
{
	class StartCountryGuessNotification
	{
		public StartCountryGuessNotification(ICountryQuestion question, SelectionMarker marker)
		{
			Question = question;
			Marker = marker;
		}

		public ICountryQuestion Question { get; private set; }

		public SelectionMarker Marker { get; private set; }
	}
}

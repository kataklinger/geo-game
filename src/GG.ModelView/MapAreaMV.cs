using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GG.Model.Contracts.Game;
using GG.Model.Contracts.GeoData;
using GG.ModelView.Messages;

namespace GG.ModelView
{
	public class MapAreaMV : ViewModelBase
	{
		private readonly SelectionMarker _selectionMarker;
		private readonly ICountryQuestion _question;

		private bool _visible;

		public MapAreaMV(SelectionMarker selectionMarker, ICountryQuestion question, GeoBounds bounds, bool markerOnly)
		{
			_selectionMarker = selectionMarker;
			_question = question;

			_question.OnStateChanged += (s, e) => FillState = e.State;
			_selectionMarker.OnSelectedChanged += (s, e) => Selected = e;


			StartGuessCommand = new RelayCommand(() =>
			{
				if (_question.State == QuestionState.Unanswered && !_selectionMarker.Selected)
					MessengerInstance.Send(new StartCountryGuessNotification(_question, _selectionMarker));
			});

			Fill = new CountryAreaFill(question.Country.LargeFlag, bounds, question.Country.Bounds, markerOnly);

			ZIndex = question.Country.ZIndex;
			Visible = true;
		}

		public RelayCommand StartGuessCommand { get; private set; }

		public CountryAreaFill Fill { get; private set; }

		public int ZIndex { get; private set; }

		public bool Visible
		{
			get { return _visible; }
			set
			{
				if (_visible != value)
				{
					_visible = value;

					RaisePropertyChanged();
				}
			}
		}

		private bool Selected
		{
			get { return Fill.Selected; }
			set
			{
				if (Fill.Selected != value)
				{
					Fill.Selected = value;

					RaisePropertyChanged("Fill");
				}
			}
		}

		private QuestionState FillState
		{
			get { return Fill.State; }
			set
			{
				if (Fill.State != value)
				{
					Fill.State = value;

					RaisePropertyChanged("Fill");
				}
			}
		}
	}
}

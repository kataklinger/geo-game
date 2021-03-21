using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using GG.Model.Contracts.Game;
using GG.Model.Contracts.Infrastructure;
using GG.ModelView.Messages;

namespace GG.ModelView
{
	public class MapGameMV : GameMV
	{
		private readonly IImageDataProvider _imageDataProvider;

		private IList<GameAnswerMV> _answers;

		private IList<MapAreaMV> _areas;

		private double _width;
		private double _height;

		private SelectionMarker _marker;

		public MapGameMV(IAppSettings settings, IImageDataProvider imageDataProvider)
			: base(settings)
		{
			_imageDataProvider = imageDataProvider;

			MessengerInstance.Register<StartCountryGuessNotification>(this, n =>
			{
				if (Game != null)
				{
					Marker = n.Marker;
					Game.AskQuestion(n.Question);

					Answers = Game.Answers
						.Select(o => new GameAnswerMV(_imageDataProvider, n.Question, (ICountryAnswer)o))
						.ToList();
				}
			});

			MessengerInstance.Register<CountryGuessNotification>(this, n =>
			{
				if (Game != null)
				{
					Game.AnswerQuestion(n.Question, n.Answer);

					Marker = null;
					Answers = null;
				}
			});

			CancelGuessCommand = new RelayCommand(() =>
			{
				Marker = null;
				Answers = null;
			});
		}

		public IList<GameAnswerMV> Answers
		{
			get { return _answers; }
			protected set
			{
				if (_answers != value)
				{
					_answers = value;

					RaisePropertyChanged();
				}
			}
		}

		public IList<MapAreaMV> Areas
		{
			get { return _areas; }
			private set
			{
				if (_areas != value)
				{
					_areas = value;

					RaisePropertyChanged();
				}
			}
		}

		public double Width
		{
			get { return _width; }
			private set
			{
				if (_width != value)
				{
					_width = value;

					RaisePropertyChanged();
				}
			}
		}

		public double Height
		{
			get { return _height; }
			private set
			{
				if (_height != value)
				{
					_height = value;

					RaisePropertyChanged();
				}
			}
		}

		protected override bool CanHandleGame(IGame game)
		{
			return game is IMapGame;
		}

		protected override void HandleNewGame(bool freshStart)
		{
			Answers = null;
			Marker = null;

			var scale = Game.Questions
				.Cast<ICountryQuestion>()
				.Min(o =>
				{
					var imageSize = _imageDataProvider.GetImageSize(o.Country.LargeFlag);

					if (imageSize == null)
						return int.MaxValue;

					return Math.Min(
					  imageSize.Width / (o.Country.Bounds.Right - o.Country.Bounds.Left),
					  imageSize.Height / (o.Country.Bounds.Bottom - o.Country.Bounds.Top));
				});

			var bounds = Game.Questions
				.Cast<ICountryQuestion>()
				.Select(o => o.Country.Bounds);

			var left = bounds
				.Select(b => b.Left)
				.Min();

			var right = bounds
				.Select(b => b.Right)
				.Max();

			var top = bounds
				.Select(b => b.Top)
				.Min();

			var bottom = bounds
				.Select(b => b.Bottom)
				.Max();

			var zMax = Game.Questions
				.Cast<ICountryQuestion>()
				.Max(o => o.Country.ZIndex) + 1;

			Areas = Game.Questions
				.Cast<ICountryQuestion>()
				.SelectMany(o =>
				{
					var selectionMark = new SelectionMarker();

					var areas = o.Country.GeomentryData
						.Where(g => !g.Ignored)
						.Select(g => new PolygonMapAreaMV(selectionMark, o, g, left, top, scale) as MapAreaMV);

					if (o.Country.IsSmall)
						return Enumerable
							.Repeat(new CircleMapAreaMV(selectionMark, o, left, top, scale), 1)
							.Union(areas)
							.ToList();

					return areas
						.ToList();
				})
				.OrderBy(a => a.ZIndex + (a.Fill.MarkerOnly ? zMax : 0))
				.ToList();

			Width = (right - left) * scale;
			Height = (bottom - top) * scale;
		}

		protected override void HandleEndGame()
		{
			Marker = null;
			Areas = null;
			Answers = null;
		}

		protected override IList<EndGameDetailsMV> AnswersDetails
		{
			get
			{
				return Game.Questions
					.Cast<ICountryQuestion>()
					.OrderBy(o => o.AnswerOrder)
					.Select(o => new EndGameDetailsMV(o.Country.AdministrativeName, o.Country.SmallFlag,
						((ICountryAnswer)o.Answer).Country.AdministrativeName, ((ICountryAnswer)o.Answer).Country.SmallFlag,
						o.State == QuestionState.Correct))
					.ToList();
			}
		}

		private SelectionMarker Marker
		{
			get { return _marker; }
			set
			{
				if (_marker != value)
				{
					if (_marker != null)
						_marker.Selected = false;

					_marker = value;

					if (_marker != null)
						_marker.Selected = true;
				}
			}
		}
	}
}

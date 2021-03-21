using System;
using GG.Model.Contracts.Game;
using GG.Model.Contracts.GeoData;

namespace GG.ModelView
{
	public class CountryAreaFill
	{
		public CountryAreaFill(Uri flag, GeoBounds areaBounds, GeoBounds countryBounds, bool markerOnly)
		{
			Flag = flag;

			X = areaBounds.Left - countryBounds.Left;
			Y = areaBounds.Top - countryBounds.Top;

			Width = areaBounds.Right - areaBounds.Left;
			Height = areaBounds.Bottom - areaBounds.Top;

			CountryWidth = countryBounds.Right - countryBounds.Left;
			CountryHeight = countryBounds.Bottom - countryBounds.Top;

			MarkerOnly = markerOnly;
		}

		public bool Selected { get; set; }

		public QuestionState State { get; set; }

		public Uri Flag { get; private set; }

		public double X { get; private set; }
		public double Y { get; private set; }

		public double Width { get; private set; }
		public double Height { get; private set; }

		public double CountryWidth { get; private set; }
		public double CountryHeight { get; private set; }

		public bool MarkerOnly { get; private set; }
	}
}

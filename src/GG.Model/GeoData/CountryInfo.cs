using System;
using System.Collections.Generic;
using System.Linq;
using Geo;
using Geo.Abstractions.Interfaces;
using Geo.Geometries;
using GG.Model.Contracts.GeoData;
using MoreLinq;

namespace GG.Model.GeoData
{
	class CountryInfo : ICountryInfo
	{
		private const double SmallCountryAreaThreshold = 5968710195.0;

		public CountryInfo(Continent continent, string administrativeName, IGeometry geomentryData,
			Uri smallFlag, Uri largeFlag,
			bool longitudeWrap, int zIndex, double toleranceLow, double toleranceHi)
		{
			Continent = continent;
			AdministrativeName = administrativeName;

			if (geomentryData is Polygon)
			{
				var delta = ((Polygon)geomentryData).GetArea().Value;
				GeomentryData = new List<GeoPolygon> { ConvertPolygon((Polygon)geomentryData, false, longitudeWrap) };
			}
			else if (geomentryData is MultiPolygon)
			{
				var polygons = ((MultiPolygon)geomentryData).Geometries
					.Cast<Polygon>()
					.Where(p => p.GetArea().Value > 0)
					.Select(p => new { Polygon = p, Area = p.GetArea().Value, Bounds = p.GetBounds() })
					.ToList();

				var countryBounds = geomentryData.GetBounds();
				var area = polygons.MaxBy(a => a.Area);
				var dist = Math.Sqrt(
						Math.Pow(countryBounds.MaxLon - countryBounds.MinLon - area.Bounds.MaxLon + area.Bounds.MinLon, 2) +
						Math.Pow(countryBounds.MaxLat - countryBounds.MinLat - area.Bounds.MaxLat + area.Bounds.MinLat, 2)
					);

				var total = polygons.Sum(a => a.Area);

				GeomentryData = polygons
					.Select(p => ConvertPolygon(p.Polygon, p.Area / total < (toleranceLow + toleranceHi * GetDistance(area.Bounds, p.Bounds) / dist), longitudeWrap))
					.ToList();
			}
			else
				GeomentryData = new List<GeoPolygon>();

			var bounds = GeomentryData.Where(p => !p.Ignored);
			Bounds = new GeoBounds(
				bounds.Min(p => p.Bounds.Left),
				bounds.Min(p => p.Bounds.Top),
				bounds.Max(p => p.Bounds.Right),
				bounds.Max(p => p.Bounds.Bottom));

			SmallFlag = smallFlag;
			LargeFlag = largeFlag;

			Borders = new List<IBorderInfo>();

			ZIndex = zIndex;

			IsSmall = GeomentryData.Max(g => g.Area) < SmallCountryAreaThreshold;
		}

		private double GetDistance(Envelope p1, Envelope p2)
		{
			var dist = -1.0;
			if ((p1.MaxLat >= p2.MinLat && p1.MaxLat <= p2.MaxLat) ||
				(p1.MinLat >= p2.MinLat && p1.MinLat <= p2.MaxLat) ||
				(p2.MaxLat >= p1.MinLat && p2.MaxLat <= p1.MaxLat) ||
				(p2.MinLat >= p1.MinLat && p2.MinLat <= p1.MaxLat))
			{
				dist = Math.Abs(p1.MaxLon - p2.MaxLon);
				dist = Math.Min(dist, Math.Abs(p1.MaxLon - p2.MinLon));
				dist = Math.Min(dist, Math.Abs(p1.MinLon - p2.MaxLon));
				dist = Math.Min(dist, Math.Abs(p1.MinLon - p2.MinLon));
			}

			if ((p1.MaxLon >= p2.MinLon && p1.MaxLon <= p2.MaxLon) ||
				(p1.MinLon >= p2.MinLon && p1.MinLon <= p2.MaxLon) ||
				(p2.MaxLon >= p1.MinLon && p2.MaxLon <= p1.MaxLon) ||
				(p2.MinLon >= p1.MinLon && p2.MinLon <= p1.MaxLon))
			{
				if (dist >= 0)
					return 0;

				dist = Math.Abs(p1.MaxLat - p2.MaxLat);
				dist = Math.Min(dist, Math.Abs(p1.MaxLat - p2.MinLat));
				dist = Math.Min(dist, Math.Abs(p1.MinLat - p2.MaxLat));
				dist = Math.Min(dist, Math.Abs(p1.MinLat - p2.MinLat));

				return dist;
			}

			var x = Math.Min(Math.Abs(p1.MaxLon - p2.MinLon), Math.Abs(p1.MinLon - p2.MaxLon));
			var y = Math.Min(Math.Abs(p1.MaxLat - p2.MinLat), Math.Abs(p1.MinLat - p2.MaxLat));

			return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
		}

		public Continent Continent { get; private set; }
		public string AdministrativeName { get; private set; }

		public IList<GeoPolygon> GeomentryData { get; private set; }
		public GeoBounds Bounds { get; private set; }

		public Uri LargeFlag { get; private set; }
		public Uri SmallFlag { get; private set; }

		public IList<IBorderInfo> Borders { get; private set; }

		public int ZIndex { get; private set; }

		public bool IsSmall { get; private set; }

		public void AddBorder(CountryInfo neighbor, bool hasLandBorder, bool hasMaritimeBorder, bool excluded)
		{
			Borders.Add(new BorderInfo(neighbor, hasLandBorder, hasMaritimeBorder, excluded));
		}

		private GeoPolygon ConvertPolygon(Polygon source, bool ignored, bool wrapNegative)
		{
			var shell = ConvertCoordinateSequence(source.Shell.Coordinates, wrapNegative);
			var srcArea = source.GetArea().Value;

			return new GeoPolygon(
				shell,
				source.Holes.Select(h => ConvertCoordinateSequence(h.Coordinates, wrapNegative)).ToList(),
				new GeoBounds(shell.Min(c => c.X), shell.Min(c => c.Y), shell.Max(c => c.X), shell.Max(c => c.Y)),
				srcArea, ignored);
		}

		private IList<GeoCoordinates> ConvertCoordinateSequence(CoordinateSequence source, bool wrapNegative)
		{
			return source.Select(c => new GeoCoordinates(180 + (wrapNegative && c.Longitude < 0 ? 360 + c.Longitude : c.Longitude), 180 - c.Latitude)).ToList();
		}
	}
}

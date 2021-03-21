using System.Collections.Generic;

namespace GG.Model.Contracts.GeoData
{
	public class GeoPolygon
	{
		public IList<GeoCoordinates> Shell { get; private set; }
		public IList<IList<GeoCoordinates>> Holes { get; private set; }

		public GeoBounds Bounds { get; private set; }
		public double Area { get; private set; }

		public bool Ignored { get; private set; }

		public GeoPolygon(IList<GeoCoordinates> shell, IList<IList<GeoCoordinates>> holes, GeoBounds bounds, double area, bool ignored)
		{
			Shell = shell;
			Holes = holes;

			Bounds = bounds;
			Area = area;

			Ignored = ignored;
		}
	}
}


namespace GG.Model.Contracts.GeoData
{
	public class GeoCoordinates
	{
		public double X { get; private set; }
		public double Y { get; private set; }

		public GeoCoordinates(double x, double y)
		{
			X = x;
			Y = y;
		}
	}
}

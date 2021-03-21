using System;
using System.Collections.Generic;
using System.Linq;
using GG.Model.Contracts.Game;
using GG.Model.Contracts.GeoData;

namespace GG.ModelView
{
	public class PolygonMapAreaMV : MapAreaMV
	{
		public PolygonMapAreaMV(SelectionMarker selectionMarker, ICountryQuestion question, GeoPolygon polygon, double gloablLeft, double gloablTop, double gloablScale)
			: base(selectionMarker, question, polygon.Bounds, false)
		{
			Points = polygon.Shell
				.Select(c => Tuple.Create((c.X - gloablLeft) * gloablScale, (c.Y - gloablTop) * gloablScale))
				.ToList();
		}

		public IList<Tuple<double, double>> Points { get; private set; }
	}
}

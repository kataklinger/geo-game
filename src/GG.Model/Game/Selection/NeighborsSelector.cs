using System;
using System.Collections.Generic;
using System.Linq;
using GG.Model.Contracts.Game;
using GG.Model.Contracts.Game.Options;
using GG.Model.Contracts.GeoData;
using GG.Model.Contracts.Infrastructure;

namespace GG.Model.Game.Selection
{
	class NeighborsSelector : ICountrySelector
	{
		private static Random _random = new Random();

		private readonly IAppSettings _settings;
		private readonly ICountryCollection _collection;

		public NeighborsSelector(IAppSettings settings, ICountryCollection collection)
		{
			_settings = settings;
			_collection = collection;
		}

		public string Name { get { return "Neighbors"; } }

		public IOptions CreateOptions()
		{
			return new ContinentSelectorOptions(_settings, _collection, ContinentSelectorFlags.MaritimeBorders | ContinentSelectorFlags.LimitByNeighbors | ContinentSelectorFlags.RangeLimits);
		}

		public IList<ICountryInfo> Generate(IOptions options)
		{
			var selOptions = options as ContinentSelectorOptions;

			var selected = _collection.Countries
				.Where(c => selOptions.Continent == Continent.Unspecified || c.Continent == selOptions.Continent)
				.Select(c => Enumerable
					.Repeat(c, 1)
					.Union(c.Borders
						.Where(b => (b.HasLandBorder || (b.HasMaritimeBorder && selOptions.MaritimeBorders)) && (selOptions.IgnoreBorderExlusion || !b.Excluded))
						.Select(b => new { Country = b.Neighbor, Order = _random.Next() })
						.OrderBy(i => i.Order)
						.Select(i => i.Country))
					.Take(selOptions.MaxCountryCount)
					.ToList())
				.Where(r => r.Count >= selOptions.MinCountryCount && r.Count <= selOptions.MaxCountryCount)
				.ToList();

			return (selected.Any()
				? selected.ElementAt(_random.Next(0, selected.Count))
				: Enumerable.Empty<ICountryInfo>())
				.ToList();
		}
	}
}

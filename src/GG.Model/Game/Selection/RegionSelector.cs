using System;
using System.Collections.Generic;
using System.Linq;
using GG.Model.Contracts.Game;
using GG.Model.Contracts.Game.Options;
using GG.Model.Contracts.GeoData;
using GG.Model.Contracts.Infrastructure;

namespace GG.Model.Game.Selection
{
	class RegionSelector : ICountrySelector
	{
		private static Random _random = new Random();

		private readonly IAppSettings _settings;
		private readonly ICountryCollection _collection;

		public RegionSelector(IAppSettings settings, ICountryCollection collection)
		{
			_settings = settings;
			_collection = collection;
		}

		public string Name { get { return "Region"; } }

		public IOptions CreateOptions()
		{
			return new ContinentSelectorOptions(_settings, _collection, ContinentSelectorFlags.RangeLimits);
		}

		public IList<ICountryInfo> Generate(IOptions options)
		{
			var selOptions = options as ContinentSelectorOptions;

			var order = _collection.Countries
				.Where(c => selOptions.Continent == Continent.Unspecified || c.Continent == selOptions.Continent)
				.Select(c => new { Country = c, Order = _random.Next() })
				.OrderBy(i => i.Order)
				.Select(i => i.Country);

			int count = _random.Next(selOptions.MinCountryCount, selOptions.MaxCountryCount);
			foreach (var current in order)
			{
				HashSet<ICountryInfo> selected = new HashSet<ICountryInfo> { current };
				Select(selected, current, count, selOptions.Continent, selOptions.IgnoreBorderExlusion);

				if (selected.Count >= selOptions.MinCountryCount && selected.Count <= selOptions.MaxCountryCount)
					return selected.ToList();
			}

			return Enumerable.Empty<ICountryInfo>().ToList();
		}

		private bool Select(HashSet<ICountryInfo> selected, ICountryInfo current, int count, Continent continent, bool ignoreExlusion)
		{
			var next = current.Borders
				.Where(b => b.HasLandBorder && (ignoreExlusion || !b.Excluded) && !selected.Contains(b.Neighbor) && (continent == Continent.Unspecified || b.Neighbor.Continent == continent))
				.Select(b => new { Country = b.Neighbor, Order = _random.Next() })
				.OrderBy(i => i.Order)
				.Take(_random.Next(1, count - selected.Count + 1))
				.Select(i => i.Country)
				.ToList();

			selected.UnionWith(next);
			if (selected.Count > count)
				return true;

			foreach (var item in next)
			{
				if (Select(selected, item, count, continent, ignoreExlusion))
					return true;
			}

			return false;
		}
	}
}

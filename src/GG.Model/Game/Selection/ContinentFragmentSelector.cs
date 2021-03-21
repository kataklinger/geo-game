using System;
using System.Collections.Generic;
using System.Linq;
using GG.Model.Contracts.Game;
using GG.Model.Contracts.Game.Options;
using GG.Model.Contracts.GeoData;
using GG.Model.Contracts.Infrastructure;

namespace GG.Model.Game.Selection
{
	class ContinentFragmentSelector : ICountrySelector
	{
		private static Random _random = new Random();

		private readonly IAppSettings _settings;
		private readonly ICountryCollection _collection;

		public ContinentFragmentSelector(IAppSettings settings, ICountryCollection collection)
		{
			_settings = settings;
			_collection = collection;
		}

		public string Name
		{
			get { return "Continent Fragment"; }
		}

		public IOptions CreateOptions()
		{
			return new ContinentSelectorOptions(_settings, _collection, ContinentSelectorFlags.RangeLimits | ContinentSelectorFlags.LimitByContinent);
		}

		public IList<ICountryInfo> Generate(IOptions options)
		{
			var selOptions = options as ContinentSelectorOptions;

			var continent = selOptions.Continent != Continent.Unspecified
				? selOptions.Continent
				: Enum.GetValues(typeof(Continent))
					.Cast<Continent>()
					.Where(c => selOptions.GetContryCountByContinent(c) > selOptions.MinCountryCount)
					.Select(c => new { Continent = c, Order = _random.Next() })
					.OrderBy(i => i.Order)
					.Select(i => i.Continent)
					.FirstOrDefault();

			return _collection.Countries
				.Where(c => c.Continent == continent)
				.Select(c => new { Country = c, Order = _random.Next() })
				.OrderBy(i => i.Order)
				.Select(i => i.Country)
				.Take(_random.Next(selOptions.MinCountryCount, selOptions.MaxCountryCount))
				.ToList();
		}
	}
}

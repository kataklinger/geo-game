using System;
using System.Collections.Generic;
using System.Linq;
using GG.Model.Contracts.Game;
using GG.Model.Contracts.Game.Options;
using GG.Model.Contracts.GeoData;
using GG.Model.Contracts.Infrastructure;

namespace GG.Model.Game.Selection
{
	class ContinentSelector : ICountrySelector
	{
		private static Random _random = new Random();

		private readonly IAppSettings _settings;
		private readonly ICountryCollection _collection;

		public ContinentSelector(IAppSettings settings, ICountryCollection collection)
		{
			_settings = settings;
			_collection = collection;
		}

		public string Name
		{
			get { return "Continent"; }
		}

		public IOptions CreateOptions()
		{
			return new ContinentSelectorOptions(_settings, _collection, ContinentSelectorFlags.None);
		}

		public IList<ICountryInfo> Generate(IOptions options)
		{
			var continent = ((ContinentSelectorOptions)options).Continent;
			if (continent == Continent.Unspecified)
				continent = (Continent)_random.Next((int)Continent.Africa, (int)Continent.SouthAmerica);

			return _collection.Countries
				.Where(c => c.Continent == continent)
				.Select(c => new { Country = c, Order = _random.Next() })
				.OrderBy(i => i.Order)
				.Select(i => i.Country)
				.ToList();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using GG.Model.Contracts.Game.Options;
using GG.Model.Contracts.GeoData;
using GG.Model.Contracts.Infrastructure;
using GG.Model.Game.Options;

namespace GG.Model.Game.Selection
{
	class ContinentSelectorOptions : BaseOptions
	{
		private IDictionary<Continent, int>[] _counts;

		private bool _hasRange;

		public ContinentSelectorOptions(IAppSettings settings, ICountryCollection collection, ContinentSelectorFlags flags)
		{
			IgnoreBorderExlusion = (flags & ContinentSelectorFlags.IgnoreBorderExlusion) != 0;

			_counts = new IDictionary<Continent, int>[]
			{
				GetCountryCount(collection, IgnoreBorderExlusion, (flags & ContinentSelectorFlags.LimitByNeighbors) != 0, (flags & ContinentSelectorFlags.LimitByContinent) != 0, false),
				GetCountryCount(collection, IgnoreBorderExlusion, (flags & ContinentSelectorFlags.LimitByNeighbors) != 0, (flags & ContinentSelectorFlags.LimitByContinent) != 0, true)
			};

			var continent = new ChoiceOption<Continent>("continent", "Continent", Enum
				.GetValues(typeof(Continent))
				.Cast<Continent>();

			var maritime = new ToggleOption("maritime", "Use Maritime Borders", false, (flags & ContinentSelectorFlags.MaritimeBorders) != 0, p => RefreshValues());

			_optionsList.AddOption(continent);
			_optionsList.AddOption(maritime);

			if ((flags & ContinentSelectorFlags.RangeLimits) != 0)
			{
				var limit = _counts[0][Continent.Unspecified];

				var min = new RangedOption<int>("min", "Countries (min)", 1, 1, limit, p => RefreshValues());
				var max = new RangedOption<int>("max", "Countries (max)", limit, 1, limit, p => RefreshValues());

				min.Depandancies = new List<IOption> { continent, maritime, max };
				max.Depandancies = new List<IOption> { continent, maritime, min };

				_optionsList.AddOption(min);
				_optionsList.AddOption(max);

				_hasRange = true;
			}
		}

		public Continent Continent
		{
			get { return _optionsList.GetValue<Continent>("continent"); }
			set { _optionsList.SetValue<Continent>("continent", value); }
		}

		public bool MaritimeBorders
		{
			get { return _optionsList.GetValue<bool>("maritime"); }
			set { _optionsList.SetValue<bool>("maritime", value); }
		}

		public int MinCountryCount
		{
			get { return _optionsList.GetValue<int>("min"); }
			set { _optionsList.SetValue<int>("min", value); }
		}

		public int MaxCountryCount
		{
			get { return _optionsList.GetValue<int>("max"); }
			set { _optionsList.SetValue<int>("max", value); }
		}

		public bool IgnoreBorderExlusion { get; private set; }

		public int GetContryCountByContinent(Continent continent)
		{
			return _counts[1][continent];
		}

		private void RefreshValues()
		{
			if (_hasRange)
			{
				var continent = _optionsList.GetOption<ChoiceOption<Continent>>("continent");

				var min = _optionsList.GetOption<RangedOption<int>>("min");
				var max = _optionsList.GetOption<RangedOption<int>>("max");

				var maritime = _optionsList.GetOption<ToggleOption>("maritime").CurrentValue;
				var limit = _counts[maritime ? 1 : 0][continent.CurrentValue];

				max.Min = Math.Min(min.CurrentValue, limit);
				max.Max = limit;
				min.Max = Math.Min(max.CurrentValue, limit);
			}
		}

		private IDictionary<Continent, int> GetCountryCount(ICountryCollection collection, bool ignoreExlusion, bool neighbors, bool maxContinent, bool maritime)
		{
			var counts = collection.Countries
				.GroupBy(c => c.Continent)
				.Select(i => new
				{
					Continent = i.Key,
					Count = neighbors
						? i.Max(c => c.Borders
							.Where(b => (b.HasLandBorder || (b.HasMaritimeBorder && maritime)) && (!b.Excluded || ignoreExlusion))
							.Count())
						: i.Count()
				})
				.ToList();

			return counts
				.Union(Enumerable.Repeat(new { Continent = Continent.Unspecified, Count = neighbors || maxContinent ? counts.Max(c => c.Count) : collection.Countries.Count() }, 1))
				.ToDictionary(k => k.Continent, v => v.Count);
		}
	}
}

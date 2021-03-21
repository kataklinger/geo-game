using System;
using System.Collections.Generic;
using System.Linq;
using GG.Model.Contracts.Game.Options;

namespace GG.Model.Game.Options
{
	class OptionsList : IOptionsList
	{
		private Dictionary<string, IOption> _options = new Dictionary<string, IOption>();

		public IList<IOption> Options
		{
			get
			{
				return _options
					.Values
					.ToList();
			}
		}

		public void AddOption(IOption option)
		{
			_options.Add(option.Name, option);
		}

		public void AddOptions(IEnumerable<Option> options)
		{
			foreach (var o in options)
			{
				if (_options.ContainsKey(o.Name))
					throw new ArgumentException("Item with the given key already exist in collection.", "options");
			}

			foreach (var o in options)
				_options.Add(o.Name, o);
		}

		public bool RemoveOption(string name)
		{
			return _options.Remove(name);
		}

		public void RemoveOptions(IEnumerable<string> names)
		{
			foreach (var n in names)
				_options.Remove(n);
		}

		public void SetValue<ValueType>(string name, ValueType value)
		{
			GetOption<Option<ValueType>>(name).CurrentValue = value;
		}

		public ValueType GetValue<ValueType>(string name)
		{
			return GetOption<Option<ValueType>>(name).CurrentValue;
		}

		public OptionType GetOption<OptionType>(string name) where OptionType : IOption
		{
			return (OptionType)_options[name];
		}
	}
}

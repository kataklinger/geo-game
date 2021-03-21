using System.Collections.Generic;

namespace GG.Model.Contracts.Game.Options
{
	public interface IOptionsList
	{
		IList<IOption> Options { get; }

		void AddOption(IOption option);

		void SetValue<ValueType>(string name, ValueType value);
		ValueType GetValue<ValueType>(string name);
		OptionType GetOption<OptionType>(string name) where OptionType : IOption;
	}
}

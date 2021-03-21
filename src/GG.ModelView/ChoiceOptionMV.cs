using System.Collections.Generic;
using GG.Model.Contracts.Game.Options;

namespace GG.ModelView
{
	public class ChoiceOptionMV : OptionMV
	{
		public ChoiceOptionMV(IChoiceOption option)
			: base(option)
		{
		}

		public IList<object> Choices { get { return ((IChoiceOption)_option).Choices; } }
	}
}

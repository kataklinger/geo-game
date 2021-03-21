using GG.Model.Contracts.Game.Options;

namespace GG.ModelView
{
	public class RangedOptionMV : OptionMV
	{
		public RangedOptionMV(IRangedOption option)
			: base(option)
		{
			option.OnMinValueChange += (s, e) => RaisePropertyChanged("Min");
			option.OnMaxValueChange += (s, e) => RaisePropertyChanged("Max");
		}

		public object Min { get { return ((IRangedOption)_option).Min; } }
		public object Max { get { return ((IRangedOption)_option).Max; } }
	}
}

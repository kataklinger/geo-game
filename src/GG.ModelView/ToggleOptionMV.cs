using GG.Model.Contracts.Game.Options;

namespace GG.ModelView
{
	public class ToggleOptionMV : OptionMV
	{
		public ToggleOptionMV(IToggleOption option)
			: base(option)
		{
			option.OnEnabledChange += (s, e) => RaisePropertyChanged("Enabled");
		}

		public bool Enabled { get { return ((IToggleOption)_option).Enabled; } }
	}
}

using GalaSoft.MvvmLight;
using GG.Model.Contracts.Game.Options;

namespace GG.ModelView
{
	public class OptionMV : ViewModelBase
	{
		protected readonly IOption _option;

		public OptionMV(IOption option)
		{
			_option = option;
			_option.OnOptionValueChange += (s, e) => RaisePropertyChanged("Value");

			Name = _option.Display;
		}

		public string Name { get; private set; }

		public object Value
		{
			get { return _option.Value; }
			set { _option.Value = value; }
		}
	}
}

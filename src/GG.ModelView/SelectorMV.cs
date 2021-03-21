using GalaSoft.MvvmLight;
using GG.Model.Contracts.Game;

namespace GG.ModelView
{
	public class SelectorMV : ViewModelBase
	{
		public SelectorMV(ICountrySelector selector)
		{
			Selector = selector;
		}

		public string Name { get { return Selector.Name; } }

		public ICountrySelector Selector { get; private set; }
	}
}

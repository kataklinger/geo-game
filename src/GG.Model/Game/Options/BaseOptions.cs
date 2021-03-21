using GG.Model.Contracts.Game.Options;

namespace GG.Model.Game.Options
{
	public abstract class BaseOptions : IOptions
	{
		protected IOptionsList _optionsList = new OptionsList();

		public IOptionsList OptionsList { get { return _optionsList; } }
	}
}

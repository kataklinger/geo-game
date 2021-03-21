using System;

namespace GG.Model.Contracts.Game.Options
{
	public interface IRangedOption : IOption
	{
		event EventHandler<IOption> OnMinValueChange;
		event EventHandler<IOption> OnMaxValueChange;

		object Min { get; set; }
		object Max { get; set; }
	}
}

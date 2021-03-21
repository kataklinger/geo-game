
using System;

namespace GG.Model.Contracts.Game.Options
{
	public interface IToggleOption : IOption
	{
		event EventHandler<IOption> OnEnabledChange;

		bool Enabled { get; set; }
	}
}

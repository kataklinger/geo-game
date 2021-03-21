using System;
using System.Collections.Generic;

namespace GG.Model.Contracts.Game.Options
{
	public interface IOption
	{
		event EventHandler<IOption> OnOptionValueChange;

		string Name { get; }
		string Display { get; }

		object Value { get; set; }

		IList<IOption> Depandancies { get; set; }
	}
}

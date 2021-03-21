using System;
using System.Collections.Generic;
using System.Linq;
using GG.Model.Contracts.Game.Options;

namespace GG.Model.Game.Options
{
	class ChoiceOption<ValueType> : Option<ValueType>, IChoiceOption
	{
		private Action<ChoiceOption<ValueType>> _reevaluate;

		public ChoiceOption(string name, string display, IEnumerable<ValueType> choices, ValueType value, Action<ChoiceOption<ValueType>> reevaluate)
			: base(name, display, value)
		{
			CurrentChoices = choices.ToList();

			if (!CurrentChoices.Contains(CurrentValue))
				CurrentValue = CurrentChoices.FirstOrDefault();

			_reevaluate = reevaluate;
		}

		public ChoiceOption(string name, string display, IEnumerable<ValueType> choices, ValueType value)
			: this(name, display, choices, value, null) { }

		public ChoiceOption(string name, string display, IEnumerable<ValueType> choices, Action<ChoiceOption<ValueType>> reevaluate)
			: this(name, display, choices, choices.FirstOrDefault(), reevaluate) { }

		public ChoiceOption(string name, string display, IEnumerable<ValueType> choices)
			: this(name, display, choices, choices.FirstOrDefault()) { }

		public IList<ValueType> CurrentChoices { get; private set; }

		public IList<object> Choices
		{
			get
			{
				return CurrentChoices
					.Cast<object>()
					.ToList();
			}
		}

		protected override void Reevaluate()
		{
			if (_reevaluate != null)
				_reevaluate(this);

			if (!CurrentChoices.Contains(CurrentValue))
				CurrentValue = CurrentChoices.FirstOrDefault();
		}
	}
}

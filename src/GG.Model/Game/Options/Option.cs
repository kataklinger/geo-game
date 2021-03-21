using System;
using System.Collections.Generic;
using GG.Model.Contracts.Game.Options;

namespace GG.Model.Game.Options
{
	abstract class Option : IOption
	{
		public event EventHandler<IOption> OnOptionValueChange;

		public string Name { get; private set; }
		public string Display { get; private set; }

		public abstract object Value { get; set; }

		protected IList<IOption> _depandancies;

		public Option(string name, string display)
		{
			Name = name;
			Display = display;
		}

		public IList<IOption> Depandancies
		{
			get { return _depandancies; }
			set
			{
				if (_depandancies != null)
				{
					foreach (var dep in _depandancies)
						dep.OnOptionValueChange -= HandleOptionValueChange;
				}

				_depandancies = value;
				if (_depandancies != null)
				{
					foreach (var dep in _depandancies)
						dep.OnOptionValueChange += HandleOptionValueChange;
				}

			}
		}

		protected abstract void Reevaluate();

		protected void RaiseOptionValueChange()
		{
			var evt = OnOptionValueChange;
			if (evt != null)
				evt(this, this);
		}

		private void HandleOptionValueChange(object sender, IOption args)
		{
			Reevaluate();
		}
	}

	abstract class Option<ValueType> : Option
	{
		private ValueType _currentValue;

		public Option(string name, string display, ValueType value)
			: base(name, display)
		{
			_currentValue = value;
		}

		public Option(string name, string display)
			: this(name, display, default(ValueType)) { }

		public override object Value
		{
			get { return CurrentValue; }
			set { CurrentValue = (ValueType)value; }
		}

		public ValueType CurrentValue
		{
			get { return _currentValue; }
			set
			{
				if (!_currentValue.Equals(value))
				{
					_currentValue = (ValueType)value;

					RaiseOptionValueChange();
				}
			}
		}
	}
}

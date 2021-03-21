using System;
using GG.Model.Contracts.Game.Options;

namespace GG.Model.Game.Options
{
	class RangedOption<ValueType> : Option<ValueType>, IRangedOption where ValueType : IComparable<ValueType>
	{
		private ValueType _min;
		private ValueType _max;

		private Action<RangedOption<ValueType>> _reevaluate;

		public RangedOption(string name, string display, ValueType value, ValueType min, ValueType max, Action<RangedOption<ValueType>> reevaluate)
			: base(name, display, value)
		{
			if (min.CompareTo(max) > 0)
			{
				var swap = max;
				max = min;
				min = max;
			}

			_min = min;
			_max = max;

			if (min.CompareTo(value) > 0)
				value = min;
			if (value.CompareTo(max) > 0)
				value = max;

			CurrentValue = value;

			_reevaluate = reevaluate;
		}

		public RangedOption(string name, string display, ValueType min, ValueType max, Action<RangedOption<ValueType>> reevaluate)
			: this(name, display, min, min, max, reevaluate) { }

		public RangedOption(string name, string display, ValueType value, ValueType min, ValueType max)
			: this(name, display, value, min, max, null) { }

		public RangedOption(string name, string display, ValueType min, ValueType max)
			: this(name, display, min, min, max, null) { }

		public event EventHandler<IOption> OnMinValueChange;
		public event EventHandler<IOption> OnMaxValueChange;

		public object Min
		{
			get { return CurrentMin; }
			set { CurrentMin = (ValueType)value; }
		}

		public object Max
		{
			get { return CurrentMax; }
			set { CurrentMax = (ValueType)value; }
		}

		public ValueType CurrentMin
		{
			get { return _min; }
			set
			{
				if (!_min.Equals(value))
				{
					_min = value;

					var evt = OnMinValueChange;
					if (evt != null)
						evt(this, this);
				}
			}
		}

		public ValueType CurrentMax
		{
			get { return _max; }
			set
			{
				if (!_max.Equals(value))
				{
					_max = value;

					var evt = OnMaxValueChange;
					if (evt != null)
						evt(this, this);
				}
			}
		}

		protected override void Reevaluate()
		{
			if (_reevaluate != null)
				_reevaluate(this);

			if (CurrentMin.CompareTo(CurrentValue) > 0)
				CurrentValue = CurrentMin;
			if (CurrentValue.CompareTo(CurrentMax) > 0)
				CurrentValue = CurrentMax;
		}
	}
}

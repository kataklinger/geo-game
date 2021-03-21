using System;
using GG.Model.Contracts.Game.Options;

namespace GG.Model.Game.Options
{
	class ToggleOption : Option<bool>, IToggleOption
	{
		private bool _enabled;

		private Action<ToggleOption> _reevaluate;

		public ToggleOption(string name, string display, bool value, bool enabled, Action<ToggleOption> reevaluate)
			: base(name, display, value)
		{
			_enabled = enabled;

			_reevaluate = reevaluate;
		}

		public ToggleOption(string name, string display, bool value, Action<ToggleOption> reevaluate)
			: this(name, display, value, true, reevaluate) { }

		public ToggleOption(string name, string display, Action<ToggleOption> reevaluate)
			: this(name, display, false, true, reevaluate) { }

		public ToggleOption(string name, string display, bool value, bool enabled)
			: this(name, display, value, enabled, null) { }

		public ToggleOption(string name, string display, bool value)
			: this(name, display, value, true, null) { }

		public ToggleOption(string name, string display)
			: this(name, display, false, true, null) { }

		public event EventHandler<IOption> OnEnabledChange;

		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				if (_enabled != value)
				{
					_enabled = value;

					var evt = OnEnabledChange;
					if (evt != null)
						evt(this, this);
				}
			}
		}

		protected override void Reevaluate()
		{
			if (_reevaluate != null)
				_reevaluate(this);
		}
	}
}

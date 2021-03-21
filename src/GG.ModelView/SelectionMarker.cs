using System;

namespace GG.ModelView
{
	public class SelectionMarker
	{
		private bool _selected;

		public event EventHandler<bool> OnSelectedChanged;

		public bool Selected
		{
			get { return _selected; }
			set
			{
				if (_selected != value)
				{
					_selected = value;

					var evt = OnSelectedChanged;
					if (evt != null)
						evt(this, _selected);
				}
			}
		}
	}
}

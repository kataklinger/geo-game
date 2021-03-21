using System.Windows;
using GG.ModelView;

namespace GG.View.Support
{
	public class OptionTemplateSelector : DataTemplateSelector
	{
		public DataTemplate ChoiceOption { get; set; }
		public DataTemplate RangedOption { get; set; }
		public DataTemplate ToggleOption { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (item is ChoiceOptionMV)
				return ChoiceOption;
			else if (item is RangedOptionMV)
				return RangedOption;
			else if(item is ToggleOptionMV)
				return ToggleOption;

			return base.SelectTemplate(item, container);
		}
	}
}

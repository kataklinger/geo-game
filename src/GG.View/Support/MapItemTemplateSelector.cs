using System.Windows;
using GG.ModelView;

namespace GG.View.Support
{
	public class MapItemTemplateSelector : DataTemplateSelector
	{
		public DataTemplate PolygonItem { get; set; }
		public DataTemplate CircleItem { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (item is PolygonMapAreaMV)
				return PolygonItem;
			else if (item is CircleMapAreaMV)
				return CircleItem;

			return base.SelectTemplate(item, container);
		}
	}
}

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using GG.ModelView;
using GG.View.Converters;

namespace GG.View.Support
{
	public class CanvasItemsControl : ItemsControl
	{
		private static PosValueConverter _converter = new PosValueConverter();

		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			if (item is CircleMapAreaMV)
			{
				FrameworkElement contentitem = element as FrameworkElement;

				Binding leftBinding = new Binding() { Path = new PropertyPath("Left"), Converter = _converter };
				Binding topBinding = new Binding() { Path = new PropertyPath("Top"), Converter = _converter };
				contentitem.SetBinding(Canvas.LeftProperty, leftBinding);
				contentitem.SetBinding(Canvas.TopProperty, topBinding);
			}

			base.PrepareContainerForItemOverride(element, item);
		}
	}
}

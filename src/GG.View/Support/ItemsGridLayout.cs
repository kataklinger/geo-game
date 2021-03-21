using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GG.View.Support
{
    public class ItemsGridLayout
    {
        public static int GetGridRow(DependencyObject obj)
        {
            return (int)obj.GetValue(GridRowProperty);
        }

        public static void SetGridRow(DependencyObject obj, int value)
        {
            obj.SetValue(GridRowProperty, value);
        }

        public static readonly DependencyProperty GridRowProperty =
            DependencyProperty.RegisterAttached("GridRow", typeof(int), typeof(FrameworkElement), new PropertyMetadata(0, (s, e) =>
            {
                var presenter = GetItemsPresenter(s);
                if (presenter != null)
                {
                    Grid.SetRow(presenter, GetGridRow(s));
                }
            }));

        public static int GetGridColumn(DependencyObject obj)
        {
            return (int)obj.GetValue(GridColumnProperty);
        }

        public static void SetGridColumn(DependencyObject obj, int value)
        {
            obj.SetValue(GridColumnProperty, value);
        }

        public static readonly DependencyProperty GridColumnProperty =
            DependencyProperty.RegisterAttached("GridColumn", typeof(int), typeof(FrameworkElement), new PropertyMetadata(0, (s, e) =>
            {
                var presenter = GetItemsPresenter(s);
                if (presenter != null)
                {
                    Grid.SetColumn(presenter, GetGridColumn(s));
                }
            }));

        static FrameworkElement GetItemsPresenter(DependencyObject target)
        {
            while (target != null)
            {
                if (target is ContentPresenter)
                {
                    return target as FrameworkElement;
                }
                target = VisualTreeHelper.GetParent(target);
            }
            return null;
        }
    }
}

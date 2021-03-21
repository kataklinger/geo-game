using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GG.ModelView;
using Microsoft.Phone.Controls;

namespace GG.View
{
	public partial class MapGameV : PhoneApplicationPage
	{
		private double m_Zoom = 1;
		private double m_Width = 0;
		private double m_Height = 0;

		private Panel _mapPanel;
		private ScaleTransform _mapTransform;

		public MapGameV()
		{
			InitializeComponent();
		}

		void Page_Loaded(object sender, RoutedEventArgs e)
		{
			_mapPanel = GetItemsPanel(map);
			_mapTransform = (ScaleTransform)_mapPanel.RenderTransform;

			var ctx = (MapGameMV)DataContext;

			m_Width = ctx.Width + 120;
			m_Height = ctx.Height + 120;

			m_Zoom = Math.Max(viewport.ActualWidth / m_Width, viewport.ActualHeight / m_Height);

			_mapTransform.ScaleX = m_Zoom;
			_mapTransform.ScaleY = m_Zoom;

			map.Width = m_Width * m_Zoom;
			map.Height = m_Height * m_Zoom;

			viewport.Bounds = new Rect(0, 0, map.Width, map.Height);
		}

		private Panel GetItemsPanel(DependencyObject itemsControl)
		{
			return VisualTreeHelper.GetChild(GetVisualChild<ItemsPresenter>(itemsControl), 0) as Panel;
		}

		private static T GetVisualChild<T>(DependencyObject parent) where T : DependencyObject
		{
			T child = default(T);

			int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
			for (int i = 0; i < numVisuals; i++)
			{
				var v = VisualTreeHelper.GetChild(parent, i);
				child = v as T;
				if (child == null)
					child = GetVisualChild<T>(v);

				if (child != null)
					break;
			}

			return child;
		}

		private void ViewportControl_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
		{
		}

		private void ViewportControl_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
		{
			if (e.PinchManipulation != null)
			{
				double newWidth, newHieght;

				if (m_Width < m_Height)
				{
					newHieght = m_Height * m_Zoom * e.PinchManipulation.CumulativeScale;
					newHieght = Math.Max(viewport.ActualHeight, newHieght);
					newHieght = Math.Min(newHieght, m_Height);
					newWidth = newHieght * m_Width / m_Height;
				}
				else
				{
					newWidth = m_Width * m_Zoom * e.PinchManipulation.CumulativeScale;
					newWidth = Math.Max(viewport.ActualWidth, newWidth);
					newWidth = Math.Min(newWidth, m_Width);
					newHieght = newWidth * m_Height / m_Width;
				}

				if (newWidth < m_Width && newHieght < m_Height)
				{
					MatrixTransform transform = map.TransformToVisual(viewport) as MatrixTransform;

					Point pinchCenterOnImage = transform.Transform(e.PinchManipulation.Original.Center);
					Point relativeCenter = new Point(e.PinchManipulation.Original.Center.X / map.Width, e.PinchManipulation.Original.Center.Y / map.Height);
					Point newOriginPoint = new Point(relativeCenter.X * newWidth - pinchCenterOnImage.X, relativeCenter.Y * newHieght - pinchCenterOnImage.Y);

					viewport.SetViewportOrigin(newOriginPoint);
				}

				_mapTransform.ScaleX = map.Width / m_Width;
				_mapTransform.ScaleY = map.Width / m_Width;

				map.Width = newWidth;
				map.Height = newHieght;

				viewport.Bounds = new Rect(0, 0, newWidth, newHieght);

				e.Handled = true;
			}
		}

		private void ViewportControl_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
		{
			m_Zoom = _mapTransform.ScaleX;
		}
	}
}
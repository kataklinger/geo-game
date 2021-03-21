using System;
using System.Windows;
using System.Windows.Threading;
using GG.Model.Contracts.Infrastructure;

namespace GG.App
{
	class TimerService : ITimerService
	{
		private DispatcherTimer _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(125) };

		public void Initialize()
		{
			Deployment.Current.Dispatcher.BeginInvoke(_timer.Start);
		}

		public void Dispose()
		{
			Deployment.Current.Dispatcher.BeginInvoke(_timer.Stop);
		}

		public void Subscribe(EventHandler handler)
		{
			Deployment.Current.Dispatcher.BeginInvoke(() => _timer.Tick += handler);
		}

		public void Unsubscribe(EventHandler handler)
		{
			Deployment.Current.Dispatcher.BeginInvoke(() => _timer.Tick -= handler);
		}
	}
}

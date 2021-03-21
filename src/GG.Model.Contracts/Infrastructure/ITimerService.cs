using System;

namespace GG.Model.Contracts.Infrastructure
{
	public interface ITimerService : IDisposable
	{
		void Initialize();

		void Subscribe(EventHandler handler);
		void Unsubscribe(EventHandler handler);
	}
}

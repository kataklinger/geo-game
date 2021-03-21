using GalaSoft.MvvmLight.Command;
using GG.Model.Contracts.Infrastructure;
using GG.ModelView.Messages;

namespace GG.ModelView
{
	public class PausedGameMV : PageBaseMV
	{
		public PausedGameMV(IAppSettings settings)
			: base(settings)
		{
			ResumeCommand = new RelayCommand(() => MessengerInstance.Send(new ResumeGameNotification()));
			RestartCommand = new RelayCommand(() => MessengerInstance.Send(new RestartGameNotification(false)));
			RegenerateCommand = new RelayCommand(() => MessengerInstance.Send(new RestartGameNotification(true)));
			MenuCommand = new RelayCommand(() => MessengerInstance.Send(new StopGameNotification()));
		}

		public RelayCommand ResumeCommand { get; private set; }
		public RelayCommand RestartCommand { get; private set; }
		public RelayCommand RegenerateCommand { get; private set; }
		public RelayCommand MenuCommand { get; private set; }
	}
}

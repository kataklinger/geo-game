using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GG.Model.Contracts.Infrastructure;
using GG.ModelView.Messages;

namespace GG.ModelView
{
	public class AboutMV : ViewModelBase
	{
		private readonly IAppSettings _appSettings;

		public AboutMV(IAppSettings appSettings)
		{
			_appSettings = appSettings;

			NavigateCommand = new RelayCommand(() => MessengerInstance.Send(new WebNavigateNotification(new Uri("http://retrocloud.net/"))));
		}

		public RelayCommand NavigateCommand { get; private set; }

		public string AppVersion { get { return _appSettings.AppVersion; } }
		public string DbVersion { get { return _appSettings.DbLocalVersion.ToString(); } }
	}
}

using System;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using GG.Model.Contracts.Game;
using GG.Model.Contracts.GeoData;
using GG.Model.Contracts.Infrastructure;
using GG.Model.Game;
using GG.Model.GeoData;
using GG.ModelView;
using Microsoft.Practices.ServiceLocation;

namespace GG.App.ViewModel
{
	public class ViewModelLocator
	{
		private ManualResetEvent _initialized = new ManualResetEvent(false);

		public ViewModelLocator()
		{
			ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

			if (ViewModelBase.IsInDesignModeStatic)
			{
			}
			else
			{
				var navigationService = CreateNavigationService();
				SimpleIoc.Default.Register<INavigationService>(() => navigationService);
				SimpleIoc.Default.Register<IDialogService, DialogService>();

				SimpleIoc.Default.Register<IAppSettings, AppSettings>(true);
				SimpleIoc.Default.Register<ITimerService, TimerService>(true);
				SimpleIoc.Default.Register<IResourceDataProvider, ResourceDataProvider>(true);
				SimpleIoc.Default.Register<IImageDataProvider, ImageDataProvider>(true);
				SimpleIoc.Default.Register<ICountryCollection, CountryCollection>(true);
				SimpleIoc.Default.Register<IGameProvider, GameProvider>(true);
			}

			SimpleIoc.Default.Register<GameSelectionMV>();
			SimpleIoc.Default.Register<MapGameMV>(true);
			SimpleIoc.Default.Register<QuizGameMV>(true);
			SimpleIoc.Default.Register<EndGameMV>(true);
			SimpleIoc.Default.Register<PausedGameMV>();
			SimpleIoc.Default.Register<MainMV>();
			SimpleIoc.Default.Register<AboutMV>();
		}

		public GameSelectionMV GameSelection { get { return ServiceLocator.Current.GetInstance<GameSelectionMV>(); } }
		public MapGameMV MapGame { get { return ServiceLocator.Current.GetInstance<MapGameMV>(); } }
		public QuizGameMV QuizGame { get { return ServiceLocator.Current.GetInstance<QuizGameMV>(); } }
		public EndGameMV EndGame { get { return ServiceLocator.Current.GetInstance<EndGameMV>(); } }
		public PausedGameMV PausedGame { get { return ServiceLocator.Current.GetInstance<PausedGameMV>(); } }
		public MainMV Main { get { return ServiceLocator.Current.GetInstance<MainMV>(); } }
		public AboutMV About { get { return ServiceLocator.Current.GetInstance<AboutMV>(); } }

		public void Initialize()
		{
			Task.Run(async () =>
			{
				ServiceLocator.Current.GetInstance<ITimerService>().Initialize();
				await ServiceLocator.Current.GetInstance<ICountryCollection>().Initialize();

				_initialized.Set();
			});

			_initialized.WaitOne();
		}

		public static void Cleanup()
		{
		}

		private INavigationService CreateNavigationService()
		{
			var navigationService = new NavigationService();

			navigationService.Configure("MainPage", new Uri("/GG.App;component/MainPage.xaml", UriKind.Relative));
			navigationService.Configure("GameSelectionV", new Uri("/GG.View;component/GameSelectionV.xaml", UriKind.Relative));
			navigationService.Configure("MapGameV", new Uri("/GG.View;component/MapGameV.xaml", UriKind.Relative));
			navigationService.Configure("QuizGameV", new Uri("/GG.View;component/QuizGameV.xaml", UriKind.Relative));
			navigationService.Configure("EndGameV", new Uri("/GG.View;component/EndGameV.xaml", UriKind.Relative));
			navigationService.Configure("PausedGameV", new Uri("/GG.View;component/PausedGameV.xaml", UriKind.Relative));
			navigationService.Configure("AboutV", new Uri("/GG.View;component/AboutV.xaml", UriKind.Relative));

			return navigationService;

		}
	}
}
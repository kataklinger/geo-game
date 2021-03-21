using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using GG.Model.Contracts.Game;
using GG.Model.Contracts.Infrastructure;
using GG.ModelView;
using GG.ModelView.Messages;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace GG.App.ViewModel
{
	public class MainMV : PageBaseMV
	{
		private readonly INavigationService _navigationService;
		private readonly IDialogService _dialogService;
		private readonly IResourceDataProvider _resourceDataProvider;
		private readonly IAppSettings _settings;

		private readonly SoundEffectInstance _correctAnswerSound;
		private readonly SoundEffectInstance _incorrectAnswerSound;

		private bool _isSoundOn;
		private string _toggleSoundText;

		public MainMV(INavigationService navigationService, IDialogService dialogService, IResourceDataProvider resourceDataProvider, IAppSettings settings)
			: base(settings)
		{
			_navigationService = navigationService;
			_dialogService = dialogService;
			_settings = settings;
			_resourceDataProvider = resourceDataProvider;

			IsSoundOn = _settings.IsSoundOn;

			_correctAnswerSound = LoadSound("Assets/CorrectAnswer.wav");
			_incorrectAnswerSound = LoadSound("Assets/IncorrectAnswer.wav");

			NewGameCommand = new RelayCommand(() => _navigationService.NavigateTo("GameSelectionV"));

			ToggleSoundCommand = new RelayCommand(() =>
			{
				var sound = !IsSoundOn;
				_settings.IsSoundOn = sound;
				IsSoundOn = sound;
			});

			AboutCommand = new RelayCommand(() => _navigationService.NavigateTo("AboutV"));

			RateAppCommand = new RelayCommand(() =>
			{
				MarketplaceReviewTask task = new MarketplaceReviewTask();
				task.Show();
			});

			MessengerInstance.Register<GamePreparedNotification>(this, n =>
			{
				var game = n.Game;

				if (game is IMapGame)
					_navigationService.NavigateTo("MapGameV");
				else if (game is IQuizGame)
					_navigationService.NavigateTo("QuizGameV");
			});

			MessengerInstance.Register<EndGameNotification>(this, n => _navigationService.NavigateTo("EndGameV"));
			MessengerInstance.Register<PauseGameNotification>(this, n => _navigationService.NavigateTo("PausedGameV"));
			MessengerInstance.Register<ResumeGameNotification>(this, n => _navigationService.GoBack());
			MessengerInstance.Register<RestartGameNotification>(this, n => _navigationService.GoBack());
			MessengerInstance.Register<StopGameNotification>(this, n => _navigationService.NavigateTo("MainPage"));

			MessengerInstance.Register<AnswerNotification>(this, n =>
			{
				if (IsSoundOn)
				{
					FrameworkDispatcher.Update();
					(n.IsCorrect ? _correctAnswerSound : _incorrectAnswerSound).Play();
				}
			});

			MessengerInstance.Register<GameGenerationFailedNotification>(this, n =>
				_dialogService.ShowMessage("We're unable to generate game with given options. Please try again with different options.", "Game cannot be generated!"));

			MessengerInstance.Register<WebNavigateNotification>(this, n =>
			{
				WebBrowserTask webBrowserTask = new WebBrowserTask();
				webBrowserTask.Uri = n.Location;
				webBrowserTask.Show();
			});
		}

		public RelayCommand NewGameCommand { get; private set; }
		public RelayCommand ToggleSoundCommand { get; private set; }
		public RelayCommand AboutCommand { get; private set; }
		public RelayCommand RateAppCommand { get; private set; }

		public bool IsSoundOn
		{
			get { return _isSoundOn; }
			private set
			{
				if (_isSoundOn != value)
				{
					_isSoundOn = value;

					RaisePropertyChanged();
				}

				ToggleSoundText = _isSoundOn ? "Turn Off Sound" : "Turn On Sound";
			}
		}

		public string ToggleSoundText
		{
			get { return _toggleSoundText; }
			private set
			{
				if (_toggleSoundText != value)
				{
					_toggleSoundText = value;

					RaisePropertyChanged();
				}
			}
		}

		private SoundEffectInstance LoadSound(string path)
		{
			using (var stream = _resourceDataProvider.GetDataStream(path))
			{
				SoundEffect sound = SoundEffect.FromStream(stream);
				return sound.CreateInstance();
			}
		}
	}
}

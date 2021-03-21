using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using GG.Model.Contracts.Game;
using GG.Model.Contracts.Game.Options;
using GG.Model.Contracts.Infrastructure;
using GG.ModelView.Messages;

namespace GG.ModelView
{
	public class GameSelectionMV : PageBaseMV
	{
		private readonly IGameProvider _gameProvider;

		private readonly IList<GameDescriptionMV> _games;
		private GameDescriptionMV _currentGame;

		private IList<SelectorMV> _selectors;
		private SelectorMV _currentSelector;

		private IOptions _selectorOptions;
		private IOptions _gameOptions;

		private IList<OptionMV> _options;

		public GameSelectionMV(IAppSettings settings, IGameProvider gameProvider)
			: base(settings)
		{
			StartGameCommand = new RelayCommand(() =>
			{
				var game = _currentGame.GameDescription.CreateGame(_gameOptions, _currentSelector.Selector, _selectorOptions);
				MessengerInstance.Send(new StartGameNotification(game));
			});

			_gameProvider = gameProvider;
			_games = _gameProvider.Games
				.Select(g => new GameDescriptionMV(g))
				.ToList();

			CurrentGame = _games.FirstOrDefault();
		}

		public RelayCommand StartGameCommand { get; private set; }

		public IList<GameDescriptionMV> Games { get { return _games; } }

		public GameDescriptionMV CurrentGame
		{
			get { return _currentGame; }
			set
			{
				if (_currentGame != value)
				{
					_currentGame = value;

					RaisePropertyChanged();

					if (_currentGame != null)
					{
						_gameOptions = _currentGame.GameDescription.CreateGameOptions();

						Selectors = _currentGame.GameDescription.Selectors
							.Select(s => new SelectorMV(s))
							.ToList();
					}
					else
						Selectors = null;

					StartGameCommand.RaiseCanExecuteChanged();
				}
			}
		}

		public IList<SelectorMV> Selectors
		{
			get { return _selectors; }
			private set
			{
				if (_selectors != value)
				{
					_selectors = value;

					RaisePropertyChanged();

					CurrentSelector = _selectors != null ? _selectors.FirstOrDefault() : null;
				}
			}
		}

		public SelectorMV CurrentSelector
		{
			get { return _currentSelector; }
			set
			{
				if (_currentSelector != value)
				{
					_currentSelector = value;

					try
					{
						// list picker for some reason throws exception that selected item is not valid
						RaisePropertyChanged();
					}
					catch { }

					_selectorOptions = _currentSelector.Selector.CreateOptions();

					Options = _gameOptions.OptionsList.Options
						.Union(_selectorOptions.OptionsList.Options)
						.Select(CreateOptionMV)
						.Where(p => p != null)
						.ToList();

					StartGameCommand.RaiseCanExecuteChanged();
				}
			}
		}

		public IList<OptionMV> Options
		{
			get { return _options; }
			private set
			{
				if (_options != value)
				{
					_options = value;

					RaisePropertyChanged();
				}
			}
		}

		private OptionMV CreateOptionMV(IOption option)
		{
			if (option is IChoiceOption)
				return new ChoiceOptionMV((IChoiceOption)option);
			else if (option is IRangedOption)
				return new RangedOptionMV((IRangedOption)option);
			else if (option is IToggleOption)
				return new ToggleOptionMV((IToggleOption)option);

			return null;
		}
	}
}

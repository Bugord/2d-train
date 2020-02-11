﻿using Assets.Scripts;
using Assets.Scripts.Services;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class EndGameMenuController : PanelBase
    {
        [SerializeField] private Text _coins;
        [SerializeField] private Text _distance;
        [SerializeField] private TimerButtonController _timerButton;
        
        [SerializeField] private Button _bonusButton;
        [SerializeField] private Button _exitToMenu;
        private LevelService _levelService;
        private AdsService _adsService;
        private GameDataService _gameDataService;
        private UIService _uiService;
        private PlayGamesService _playGamesService;

        private void Awake()
        {
            _levelService = ServiceLocator.GetService<LevelService>();
            _adsService = ServiceLocator.GetService<AdsService>();
            _gameDataService = ServiceLocator.GetService<GameDataService>();
            _uiService = ServiceLocator.GetService<UIService>();
            _playGamesService = ServiceLocator.GetService<PlayGamesService>();

            _bonusButton.onClick.AddListener(_adsService.ShowBonusVideoAdvertisement);

            _timerButton.TimerEnded += delegate { _bonusButton.gameObject.SetActive(false); };
            _adsService.BonusAdvertisementUpdate += delegate(bool isReady) { _bonusButton.interactable = isReady; };
            
            _adsService.BonusCoins += GetBonus;
            _uiService.OpenEndGameMenu += Open;
            _uiService.OpenPauseMenu += () => SetActivePanel(false);
            _exitToMenu.onClick.AddListener(ExitToMainMenu);
        }

        private void Open()
        {
            InputManager.BackButton += ExitToMainMenu;
            SetEndGameData();
            SetActivePanel(true);
            _timerButton.StartTimer(false, null);
        }

        private void ExitToMainMenu()
        {
            InputManager.BackButton -= ExitToMainMenu;
            _adsService.ShowGameOverAdvertisement();
            SetActivePanel(false);
            _gameDataService.SetLastLevel(_levelService.Level);
            _gameDataService.UpdateCloudVariables();
            _playGamesService.SaveData();
            _bonusButton.gameObject.SetActive(true);
            MapGenerator.Instance.ResetGenerator();
            _levelService.UpdateService();
            _uiService.UpdateMainMenu();
            _uiService.ExitToMainMenu();
        }

        private void GetBonus()
        {
            _gameDataService.Coins *= 2;
            SetEndGameData();
            _bonusButton.gameObject.SetActive(false);
        }

        private void SetEndGameData()
        {
            _coins.text = _gameDataService.Coins.ToString();
            _distance.text = _gameDataService.Score.ToString();
        }
    }
}
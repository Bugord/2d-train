﻿using System;
using System.Collections;
using Assets.Scripts;
using Assets.Scripts.Services;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class EndGameMenuController : PanelBase
    {
        [SerializeField] private Text _coins;
        [SerializeField] private Text _distance;
        [SerializeField] private AdsTimerButton _timerButton;
        
        [SerializeField] private Button _bonusButton;
        [SerializeField] private Button _exitToMenu;
        private LevelService _levelService;
        private AdsService _adsService;
        private GameDataService _gameDataService;
        private UIService _uiService;
        private PlayGamesService _playGamesService;
        private AchievementsService _achievementsService;
        
        private void Awake()
        {
            _levelService = ServiceLocator.GetService<LevelService>();
            _adsService = ServiceLocator.GetService<AdsService>();
            _gameDataService = ServiceLocator.GetService<GameDataService>();
            _uiService = ServiceLocator.GetService<UIService>();
            _playGamesService = ServiceLocator.GetService<PlayGamesService>();
            _achievementsService = ServiceLocator.GetService<AchievementsService>();

            _bonusButton.onClick.AddListener(_adsService.ShowBonusVideoAdvertisement);

            _timerButton.TimerEnded += delegate { _bonusButton.gameObject.SetActive(false); };
            _adsService.BonusAdvertisementUpdate += delegate(bool isReady) { _bonusButton.interactable = isReady; };
            
            _adsService.BonusCoins += GetBonus;
            _uiService.OpenEndGameMenu += Open;
            _uiService.OpenPauseMenu += () => SetActivePanel(false);
            _exitToMenu.onClick.AddListener(ExitToMainMenu);
            _uiService.EndGameBackButton += ExitToMainMenu;
        }

        private void Open()
        {
            _uiService.CurrentPanel = this;
            SetEndGameData();
            SetActivePanel(true);
            
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                _bonusButton.gameObject.SetActive(false);
            }
            else
            {
                _bonusButton.gameObject.SetActive(true);
                _timerButton.StartTimer(false, null);
            }
            
            _achievementsService.IncrementAchievement(GPGSIds.achievement_every_coin_counts, _gameDataService.Coins);
            _achievementsService.IncrementAchievement(GPGSIds.achievement_startup_capital, _gameDataService.Coins);
            _achievementsService.IncrementAchievement(GPGSIds.achievement_capitalist, _gameDataService.Score/3);
            
            _achievementsService.IncrementAchievement(GPGSIds.achievement_to_the_edge_of_the_world, _gameDataService.Score);
            _achievementsService.IncrementAchievement(GPGSIds.achievement_to_the_edge_of_the_galaxy, (int)(_gameDataService.Score*0.5f));
            _achievementsService.IncrementAchievement(GPGSIds.achievement_to_the_edge_of_the_universe, (int)(_gameDataService.Score/4.2f));
        }

        private void ExitToMainMenu()
        {
            if (!CloudVariables.IsAdsRemoved())
            {
                _adsService.ShowGameOverAdvertisement();
            }
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
            _achievementsService.UnlockAchievement(GPGSIds.achievement_i_want_it_all);
        }

        private void SetEndGameData()
        {
            StartCoroutine(UpdateText(_coins, int.Parse(_coins.text), _gameDataService.Coins));
            StartCoroutine(UpdateText(_distance, int.Parse(_distance.text), _gameDataService.Score));
        }
    }
}
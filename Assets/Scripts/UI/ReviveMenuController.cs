﻿using Assets.Scripts;
using Assets.Scripts.Services;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ReviveMenuController : PanelBase
    {
        [SerializeField] private Button _reviveButton;
        [SerializeField] private AdsTimerButton _timerButton;
        [SerializeField] private Button _noThanksButton;

        private AdsService _adsService;
        private UIService _uiService;
        private AchievementsService _achievementsService;
        
        // Start is called before the first frame update
        void Awake()
        {
            _adsService = ServiceLocator.GetService<AdsService>();
            _uiService = ServiceLocator.GetService<UIService>();
            _achievementsService = ServiceLocator.GetService<AchievementsService>();

            _uiService.OpenReviveMenu += Open;
            _reviveButton.onClick.AddListener(_adsService.ShowReviveVideoAdvertisement);
            _timerButton.TimerEnded += ShowEndGameMenu;
            _noThanksButton.onClick.AddListener(ShowEndGameMenu);
            
            _adsService.ReviveAdvertisementUpdate += delegate (bool isReady) { _reviveButton.interactable = isReady; };
            _adsService.TrainRevive += ShowPauseMenu;
            _uiService.OpenEndGameMenu += delegate { SetActivePanel(false); };
        }

        private void Open()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                ShowEndGameMenu();
                return;
            }
            _uiService.CurrentPanel = this;
            SetActivePanel(true);
            _timerButton.StartTimer(false, null);
        }
        
        private void ShowEndGameMenu()
        {
            SetActivePanel(false);
            _uiService.ShowEndGameMenu();
        }

        private void ShowPauseMenu()
        {
            SetActivePanel(false);
            _uiService.SetPause();
            _achievementsService.UnlockAchievement(GPGSIds.achievement_dont_stop_me_now);
        }
    }
}

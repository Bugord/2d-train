using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Services;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainMenuController : PanelBase
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _storeButton;
    [SerializeField] private Button _rateButton;
    
    [SerializeField] private Button _achievementsButton;
    [SerializeField] private Button _leaderboardButton;

    public Text LastScore;
    public Text BestScore;
    public Text Coins;

    private AchievementsService _achievementsService;
    private LeaderBoardsService _leaderBoardsService;
    private UIService _uiService;

    private void Awake()
    {
        _achievementsService = ServiceLocator.GetService<AchievementsService>();
        _leaderBoardsService = ServiceLocator.GetService<LeaderBoardsService>();
        _uiService = ServiceLocator.GetService<UIService>();
        
        _achievementsButton.onClick.AddListener(_achievementsService.ShowAchievementsUI);
        _leaderboardButton.onClick.AddListener(_leaderBoardsService.ShowLeaderBoardUI);

        _startButton.onClick.AddListener(StartGame);
        _rateButton.onClick.AddListener(RateApp);
        _storeButton.onClick.AddListener(OpenShop);

        _uiService.OpenMainMenu += Open;
        _uiService.UpdateMainMenuData += UpdateData;
        UpdateData();
    }

    private void StartGame()
    {
        SetActivePanel(false);
        _uiService.ShowInGameUI();
    }

    private void RateApp()
    {
        Application.OpenURL("http://play.google.com/store/apps/details?id=" + Application.identifier);
    }

    private void Open()
    {
        SetActivePanel(true);
        UpdateData();
    }

    private void OpenShop()
    {
        SetActivePanel(false);
        _uiService.ShowStoreUI();
    }

    private void UpdateData()
    {
        LastScore.text = CloudVariables.ImportantValues[4].ToString();
        BestScore.text = CloudVariables.ImportantValues[0].ToString();
        Coins.text = CloudVariables.ImportantValues[1].ToString();
    }
}

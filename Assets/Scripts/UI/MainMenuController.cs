using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Services;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : PanelBase
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _shopButton;
    [SerializeField] private Button _rateButton;
    [SerializeField] private Button _coinsStoreButton;
    [SerializeField] private Button _addCoinsButton;

    [SerializeField] private Button _achievementsButton;
    [SerializeField] private Button _leaderboardButton;

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
        _uiService.OpenMainMenu += Open;
        _uiService.UpdateMainMenuData += UpdateData;
        UpdateData();
    }

    private void StartGame()
    {
        SetActivePanel(false);
        _uiService.ShowInGameUI();
    }

    private void Open()
    {
        SetActivePanel(true);
        UpdateData();
    }

    private void UpdateData()
    {
        BestScore.text = CloudVariables.ImportantValues[0].ToString();
        Coins.text = CloudVariables.ImportantValues[1].ToString();
    }
}

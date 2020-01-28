using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Services;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : PanelBase
{
    public Button StartButton;
    public Button SettingButton;
    public Button ShopButton;
    public Button RateButton;
    public Button CoinsStoreButton;
    public Button AddCoinsButton;

    [SerializeField] private Button _achievementsButton;
    [SerializeField] private Button _leaderboardButton;

    public Text BestScore;
    public Text Coins;

    private AchievementsService _achievementsService;
    private LeaderBoardsService _leaderBoardsService;

    private void Awake()
    {
        _achievementsService = ServiceLocator.GetService<AchievementsService>();
        _leaderBoardsService = ServiceLocator.GetService<LeaderBoardsService>();
        
        _achievementsButton.onClick.AddListener(_achievementsService.ShowAchievementsUI);
        _leaderboardButton.onClick.AddListener(_leaderBoardsService.ShowLeaderBoardUI);
    }
}

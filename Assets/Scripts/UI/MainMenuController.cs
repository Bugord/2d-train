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
    
    // Start is called before the first frame update
    void Start()
    {
        StartButton.onClick.AddListener(StartGame);
        _achievementsButton.onClick.AddListener(ShowAchievementsUI);
        _leaderboardButton.onClick.AddListener(ShowLeaderboardUI);
    }

    private void ShowAchievementsUI()
    {
        ServiceLocator.GetService<AchievementsService>().ShowAchievementsUI();
    }

    private void ShowLeaderboardUI()
    {
        ServiceLocator.GetService<LeaderBoardsService>().ShowLeaderBoardUI();
    }

    private void StartGame()
    {
        Debug.LogError("Start");
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}

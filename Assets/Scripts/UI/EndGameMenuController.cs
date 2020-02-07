using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Services;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

public class EndGameMenuController : PanelBase
{
    [SerializeField] private Text _coins;
    [SerializeField] private Text _distance;
    public Button ReviveButton;
    public Button BonusButton;
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

        ReviveButton.onClick.AddListener(_adsService.ShowReviveVideoAdvertisement);
        BonusButton.onClick.AddListener(_adsService.ShowBonusVideoAdvertisement);

        _adsService.BonusAdvertisementUpdate += delegate(bool isReady) { BonusButton.interactable = isReady; };
        _adsService.ReviveAdvertisementUpdate += delegate (bool isReady) { ReviveButton.interactable = isReady; };
        _adsService.BonusCoins += GetBonus;
        _uiService.OpenEndGameMenu += Open;
        _uiService.OpenPauseMenu += () => SetActivePanel(false);
        _exitToMenu.onClick.AddListener(ExitToMainMenu);
    }

    private void Open(bool canRevive)
    {
        ReviveButton.gameObject.SetActive(canRevive);
        SetEndGameData();
        SetActivePanel(true);
    }

    private void ExitToMainMenu()
    {
        SetActivePanel(false);
        _gameDataService.SetLastLevel(_levelService.Level);
        _gameDataService.UpdateCloudVariables();
        _playGamesService.SaveData();
        BonusButton.gameObject.SetActive(true);
        ReviveButton.gameObject.SetActive(true);
        MapGenerator.Instance.ResetGenerator();
        _levelService.UpdateService();
        _uiService.UpdateMainMenu();
        _uiService.ExitToMainMenu();
    }

    private void GetBonus()
    {
        _gameDataService.Coins *= 2;
        SetEndGameData();
        BonusButton.gameObject.SetActive(false);
    }

    private void SetEndGameData()
    {
        _coins.text = _gameDataService.Coins.ToString();
        _distance.text = _gameDataService.Score.ToString();
    }
}
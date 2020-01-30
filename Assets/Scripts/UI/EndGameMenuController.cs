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

    private void Awake()
    {
        _levelService = ServiceLocator.GetService<LevelService>();
        _adsService = ServiceLocator.GetService<AdsService>();
        _gameDataService = ServiceLocator.GetService<GameDataService>();

        ReviveButton.onClick.AddListener(_adsService.ShowReviveVideoAdvertisement);
        BonusButton.onClick.AddListener(_adsService.ShowBonusVideoAdvertisement);

        ReviveButton.gameObject.SetActive(false);
        BonusButton.gameObject.SetActive(false);

        _adsService.BonusAdvertisementUpdate += delegate(bool isReady) { BonusButton.interactable = isReady; };
        _adsService.ReviveAdvertisementUpdate += delegate (bool isReady) { ReviveButton.interactable = isReady; };
        _exitToMenu.onClick.AddListener(ExitToMainMenu);
    }

    public void ExitToMainMenu()
    {
        SetActivePanel(false);
        _gameDataService.SetLastLevel(_levelService.Level);
        UIManager.Instance.ExitToMainMenu();
    }

    public void SetEndGameData()
    {
        _coins.text = _gameDataService.Coins.ToString();
        _distance.text = _gameDataService.Score.ToString();
    }
}
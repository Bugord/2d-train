using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Managers;
using Assets.Scripts.Services;
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

    private void Awake()
    {
        _levelService = ServiceLocator.GetService<LevelService>();
        _adsService = ServiceLocator.GetService<AdsService>();

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
        GameData.SetLastLevel(_levelService.Level);
        UIManager.Instance.ExitToMainMenu();
    }

    public void SetEndGameData()
    {
        _coins.text = GameData.Coins.ToString();
        _distance.text = GameData.Score.ToString();
    }
}
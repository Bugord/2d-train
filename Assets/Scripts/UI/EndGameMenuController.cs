using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

public class EndGameMenuController : PanelBase
{
    [SerializeField] private Text _coins;
    [SerializeField] private Text _distance;
    public GameObject ReviveButton;
    public GameObject BonusButton;
    [SerializeField] private Button _exitToMenu;
    private LevelService _levelService;

    private void Awake()
    {
        _levelService = ServiceLocator.GetService<LevelService>();
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
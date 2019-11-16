using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameMenuController : PanelBase
{
    [SerializeField] private Text _coins;
    [SerializeField] private Text _distance;
    public GameObject ReviveButton;
    [SerializeField] private Button _exitToMenu;

    private void Start()
    {
        _exitToMenu.onClick.AddListener(ExitToMainMenu);
    }

    private void ExitToMainMenu()
    {
        SetActivePanel(false);
        UIManager.Instance.ExitToMainMenu();
    }
    public void SetEndGameData()
    {
        _coins.text = GameData.InGameCoins.ToString();
        _distance.text = GameData.LastScore.ToString();
    }
}

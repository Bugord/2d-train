using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Services;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIController : PanelBase
{
    public Button PauseButton;
    public Text Score;
    public Text Distance;

    private GameDataService _gameDataService;

    private void Awake()
    {
        _gameDataService = ServiceLocator.GetService<GameDataService>();
    }

    public void ResetFields()
    {
        Score.text = "0";
        Distance.text = "0";
        _gameDataService.ResetGame();
    }
}

using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Services;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuController : PanelBase
{
    public Button BackButton;
    public Button ResetButton;

    private GameDataService _gameDataService;

    private void Awake()
    {
        _gameDataService = ServiceLocator.GetService<GameDataService>();

        BackButton.onClick.AddListener(GoBack);
        ResetButton.onClick.AddListener(_gameDataService.ResetProgress);
    }
}

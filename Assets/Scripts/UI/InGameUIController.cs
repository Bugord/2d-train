using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Services;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIController : PanelBase
{
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Text _score;
    [SerializeField] private Text _distance;

    private GameDataService _gameDataService;
    private UIService _uiService;
    private AdsService _adsService;

    private void Awake()
    {
        _gameDataService = ServiceLocator.GetService<GameDataService>();
        _uiService = ServiceLocator.GetService<UIService>();
        _adsService = ServiceLocator.GetService<AdsService>();

        _uiService.SetActiveInGameUI += SetActive;
        _uiService.GameRestart += ResetInGameUI;
        _uiService.InGameCoinsUpdate += UpdateCoins;
        _uiService.InGameDistanceUpdate += UpdateDistance;

        _pauseButton.onClick.AddListener(_uiService.SetPause);
        
        _gameDataService.ResetGame();
    }

    void OnApplicationFocus(bool pauseStatus)
    {
        if(!pauseStatus && _uiService.IsInGame)
        {
            SetPause();
        }
    }

    
    private void ResetInGameUI()
    {
        _score.text = "0";
        _distance.text = "0";
        _gameDataService.ResetGame();
    }

    private void SetActive(bool isActive)
    {
        if (isActive)
        {
            _adsService.HideBanner();
            InputManager.BackButton += SetPause;
        }
        else
        {
            _adsService.ShowBanner();
            InputManager.BackButton -= SetPause;
        }
        SetActivePanel(isActive);
    }

    private void SetPause()
    {
        _uiService.SetPause();
    }

    private void UpdateCoins(int coins)
    {
        _score.text = coins.ToString();
    }

    private void UpdateDistance(int distance)
    {
        _distance.text = distance.ToString();
    }
}

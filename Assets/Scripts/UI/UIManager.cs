using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = System.Object;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public MainMenuController _mainMenuController;
    [SerializeField] private SettingsMenuController _settingsMenuController;
    [SerializeField] private ShopController _shopController;
    [SerializeField] private PausePanelController _pausePanelController;
    public InGameUIController _inGameUiController;
    [SerializeField] private EndGameMenuController _endGameMenuController;
    [SerializeField] private CoinsStoreController _coinsStoreController;
    [SerializeField] private ExitGamePanelController _exitGamePanelController;

    public static bool IsInGame;

    public static PanelBase previousPanel;
    public static PanelBase currentPanel;
    
    public void Start()
    {
        Instance = this;
        previousPanel = _mainMenuController;
        currentPanel = _mainMenuController;

        _mainMenuController.StartButton.onClick.AddListener(StartGame);
        _mainMenuController.SettingButton.onClick.AddListener(OpenSettings);
        _mainMenuController.ShopButton.onClick.AddListener(OpenShop);
        _inGameUiController.PauseButton.onClick.AddListener(SetPause);
        _mainMenuController.RateButton.onClick.AddListener(RateApp);
        _mainMenuController.CoinsStoreButton.onClick.AddListener(OpenCoinsStore);
        _mainMenuController.AddCoinsButton.onClick.AddListener(OpenCoinsStore);

        InputManager.BackButton += InputManagerOnBackButton;
        AdsManager.BonusCoins += AdsManagerOnBonusCoins;
        UpdateUI();
    }

    private void AdsManagerOnBonusCoins()
    {
        GameData.Coins += GameData.Coins;
        _endGameMenuController.SetEndGameData();
    }

    private void InputManagerOnBackButton()
    {
        if (currentPanel == _mainMenuController)
        {
            previousPanel = _mainMenuController;
            currentPanel = _exitGamePanelController;
            _exitGamePanelController.SetActivePanel(true);
            IsInGame = false;
        } else if (currentPanel == _pausePanelController)
        {
            previousPanel = _pausePanelController;
            currentPanel = _inGameUiController;
            _inGameUiController.SetActivePanel(true);
            _pausePanelController.SetActivePanel(false);
            IsInGame = true;
        }
        else if (currentPanel == _endGameMenuController)
        {
            
        }
        else if (currentPanel == _exitGamePanelController)
        {
            currentPanel.GoBack();
            currentPanel = _mainMenuController;
        }
        else
        {
            currentPanel.GoBack();
        }
    }

    public void UpdateUI()
    {
        _mainMenuController.BestScore.text = CloudVariables.ImportantValues[0].ToString();
        _mainMenuController.Coins.text = CloudVariables.ImportantValues[1].ToString();
        _shopController.UpdateSkins();
        if (CloudVariables.ImportantValues[2] == 1)
        {
            RemoveAdsController.DestroyButton();
        }
        GameObject.FindGameObjectsWithTag("Mask").ToList().ForEach(mask => mask.GetComponent<Image>().color = Camera.main.backgroundColor);
    }

    private void OpenCoinsStore()
    {
        previousPanel = _mainMenuController;
        currentPanel = _coinsStoreController;
        _mainMenuController.SetActivePanel(false);
        _coinsStoreController.SetActivePanel(true);
        IsInGame = false;
    }

    private void RateApp()
    {
#if UNITY_ANDROID
        Application.OpenURL("market://details?id=" + Application.identifier);
#else
        Application.OpenURL("https://play.google.com/store/apps/details?id=" + Application.identifier);
#endif
    }

    public void ShowEndGameMenu(bool canRevive = false)
    {
        _endGameMenuController.ReviveButton.SetActive(canRevive && !GameData.Revived);
        _endGameMenuController.BonusButton.SetActive(!GameData.BonusReceived);
        _endGameMenuController.SetEndGameData();
        _endGameMenuController.SetActivePanel(true);
        _inGameUiController.SetActivePanel(false);
        currentPanel = _endGameMenuController;
    }

    public void HideEndGameMenu()
    {
        _endGameMenuController.SetActivePanel(false);
    }

    private void StartGame()
    {
        previousPanel = _pausePanelController;
        currentPanel = _inGameUiController;
        _mainMenuController.SetActivePanel(false);
        _inGameUiController.SetActivePanel(true);
        IsInGame = true;
        _inGameUiController.ResetFields();
    }

    private void OpenSettings()
    {
        previousPanel = _mainMenuController;
        currentPanel = _settingsMenuController;
        _mainMenuController.SetActivePanel(false);
        _settingsMenuController.SetActivePanel(true);
        IsInGame = false;
    }

    public void OpenShop()
    {
        previousPanel = _mainMenuController;
        currentPanel = _shopController;
        _mainMenuController.SetActivePanel(false);
        _shopController.SetActivePanel(true);
        IsInGame = false;
    }

    public void SetPause()
    {
        previousPanel = _inGameUiController;
        currentPanel = _pausePanelController;
        _inGameUiController.SetActivePanel(false);
        _pausePanelController.SetActivePanel(true);
        IsInGame = false;
    }

    public void ExitToMainMenu()
    {
        LevelManager.Instance.UpdateManager();
        currentPanel = _mainMenuController;
        _pausePanelController.SetActivePanel(false);
        _mainMenuController.SetActivePanel(true);
        IsInGame = false;
        if (GameData.Score > CloudVariables.ImportantValues[0])
        {
            CloudVariables.ImportantValues[0] = GameData.Score;
            PlayGamesScript.AddScoreToLeaderboard(GPGSIds.leaderboard_high_score, CloudVariables.ImportantValues[0]);
        }

        CloudVariables.ImportantValues[1] += GameData.Coins;
        
        PlayGamesScript.Instance.SaveData();

        UpdateUI();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

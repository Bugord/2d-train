using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private MainMenuController _mainMenuController;
    [SerializeField] private SettingsMenuController _settingsMenuController;
    [SerializeField] private ShopController _shopController;
    [SerializeField] private PausePanelController _pausePanelController;
    [SerializeField] private InGameUIController _inGameUiController;

    public static bool IsInGame;

    public static PanelBase previousPanel;

    public void Start()
    {
        _mainMenuController.StartButton.onClick.AddListener(StartGame);
        _mainMenuController.SettingButton.onClick.AddListener(OpenSettings);
        _mainMenuController.ShopButton.onClick.AddListener(OpenShop);
        _inGameUiController.PauseButton.onClick.AddListener(SetPause);
        _pausePanelController.ExitToMenuButton.onClick.AddListener(ExitToMainMenu);
    }

    private void StartGame()
    {
        Debug.Log("Game Started");
        _mainMenuController.SetActivePanel(false);
        _inGameUiController.SetActivePanel(true);
        IsInGame = true;
    }

    private void OpenSettings()
    {
        previousPanel = _mainMenuController;
        _mainMenuController.SetActivePanel(false);
        _settingsMenuController.SetActivePanel(true);
        IsInGame = false;
    }

    public void OpenShop()
    {
        previousPanel = _mainMenuController;
        _mainMenuController.SetActivePanel(false);
        _shopController.SetActivePanel(true);
        IsInGame = false;
    }

    public void SetPause()
    {
        previousPanel = _inGameUiController;
        _inGameUiController.SetActivePanel(false);
        _pausePanelController.SetActivePanel(true);
        IsInGame = false;
    }

    public void ExitToMainMenu()
    {
        _pausePanelController.SetActivePanel(false);
        _mainMenuController.SetActivePanel(true);
        IsInGame = false;
    }
}

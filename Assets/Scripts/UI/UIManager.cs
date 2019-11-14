using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public MainMenuController _mainMenuController;
    [SerializeField] private SettingsMenuController _settingsMenuController;
    [SerializeField] private ShopController _shopController;
    [SerializeField] private PausePanelController _pausePanelController;
    [SerializeField] private InGameUIController _inGameUiController;
    [SerializeField] private Button _removeAdsButton;

    public static bool IsInGame;

    public static PanelBase previousPanel;
    
    public void Start()
    {
        Instance = this;
        _mainMenuController.StartButton.onClick.AddListener(StartGame);
        _mainMenuController.SettingButton.onClick.AddListener(OpenSettings);
        _mainMenuController.ShopButton.onClick.AddListener(OpenShop);
        _inGameUiController.PauseButton.onClick.AddListener(SetPause);
        _pausePanelController.ExitToMenuButton.onClick.AddListener(ExitToMainMenu);
        UpdateUI();
        
    }

    public void UpdateUI()
    {
        _mainMenuController.BestScore.text = PlayerPrefs.GetInt(GameDataFields.BestScore.ToString()).ToString();
        _mainMenuController.Coins.text = PlayerPrefs.GetInt(GameDataFields.Coins.ToString()).ToString();
    }

    private void RemoveAds()
    {
        if (PlayerPrefs.HasKey("AdsFree"))
        {
            print("Ads already removed");
        }
        else
        {
                
            PlayerPrefs.SetInt("AdsFree", 1);
            PlayerPrefs.Save();
        }
    }

    private void StartGame()
    {
        Debug.Log("Game Started");
        _mainMenuController.SetActivePanel(false);
        _inGameUiController.SetActivePanel(true);
        IsInGame = true;
        GameData.InGameCoins = 0;
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
        GameData.UpdateBestScore();
        GameData.AddCoins();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SetScore(int score)
    {
        _inGameUiController.Score.text = score.ToString();
    }
}

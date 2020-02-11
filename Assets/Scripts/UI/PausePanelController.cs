using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Services;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

public class PausePanelController : PanelBase
{
    [SerializeField] private Button _continueButton;
    public Button ExitToMenuButton;

    private UIService _uiService;
    
    void Awake()
    {
        _uiService = ServiceLocator.GetService<UIService>();
        _continueButton.onClick.AddListener(ContinueGame);
        ExitToMenuButton.onClick.AddListener(ExitToMainMenu);
        _uiService.OpenPauseMenu += Open;
    }

    private void Open()
    {
        SetActivePanel(true);
        InputManager.BackButton += ContinueGame;
    }

    private void ExitToMainMenu()
    {
        SetActivePanel(false);
        _uiService.ShowEndGameMenu();
    }

    private void ContinueGame()
    {
        InputManager.BackButton -= ContinueGame;
        SetActivePanel(false);
        _uiService.ShowInGameUI();
        _uiService.IsInGame = true;
    }
}

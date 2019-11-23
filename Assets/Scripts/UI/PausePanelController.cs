using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausePanelController : PanelBase
{
    [SerializeField] private Button _continueButton;
    public Button ExitToMenuButton;

    // Start is called before the first frame update
    void Start()
    {
        _continueButton.onClick.AddListener(GoBack);
        ExitToMenuButton.onClick.AddListener(ExitToMainMenu);
    }

    private void ExitToMainMenu()
    {
        SetActivePanel(false);
        UIManager.Instance.ShowEndGameMenu();
    }

    public override void GoBack()
    {
        UIManager.currentPanel = UIManager.Instance._inGameUiController;
        base.GoBack();
        UIManager.IsInGame = true;
    }
}

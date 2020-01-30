using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

public class ExitGamePanelController : PanelBase
{
    public Button YesButton;
    public Button NoButton;

    // Start is called before the first frame update
    void Start()
    {
        YesButton.onClick.AddListener(OnYes);
        NoButton.onClick.AddListener(OnNo);
    }

    private void OnYes()
    {
        Application.Quit();
    }

    private void OnNo()
    {
        GoBack();
        UIManager.currentPanel = UIManager.Instance._mainMenuController;
    }
}

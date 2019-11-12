using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuController : PanelBase
{
    public Button BackButton;
    public Button ResetButton;

    // Start is called before the first frame update
    void Start()
    {
        BackButton.onClick.AddListener(GoBack);
        ResetButton.onClick.AddListener(GameData.ResetProgress);
    }
}

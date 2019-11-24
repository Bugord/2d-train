using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIController : PanelBase
{
    public Button PauseButton;
    public Text Score;
    public Text Distance;

    public void ResetFields()
    {
        Score.text = "0";
        Distance.text = "0";
    }
}

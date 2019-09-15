using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelBase : MonoBehaviour
{
    [SerializeField]
    private GameObject root;

    public void SetActivePanel(bool isActive)
    {
        root.SetActive(isActive);
    }

    public void GoBack()
    {
        SetActivePanel(false);
        UIManager.previousPanel.SetActivePanel(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelBase : MonoBehaviour
{
    [SerializeField]
    private GameObject root;
    
    public void SetActivePanel(bool isActive)
    {
        root.SetActive(isActive);
    }

    public virtual void GoBack()
    {
        if (!UIManager.IsInGame)
        {
            SetActivePanel(false);
            UIManager.previousPanel.SetActivePanel(true);
            UIManager.currentPanel = UIManager.previousPanel;
        }
        else
        {
            UIManager.Instance.SetPause();
        }
    }
}

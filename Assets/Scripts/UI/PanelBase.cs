using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.UI;
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
}

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
    
    protected IEnumerator UpdateText(Text text, int oldValue, long newValue)
    {
        float t = 0;
        while (t <= 1)
        {
            t += Time.deltaTime*2;
            text.text = ((int)Mathf.Lerp(oldValue, newValue, t)).ToString();
            yield return null;
        }
    }
}

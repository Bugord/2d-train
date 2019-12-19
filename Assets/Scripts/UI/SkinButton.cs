using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinButton : MonoBehaviour
{
    public Image SkinImage;
    [field: SerializeField]
    public bool IsUnlocked { get; private set; }

    [SerializeField] private GameObject _mask;

    // Start is called before the first frame update
    void Start()
    {
        if (IsUnlocked)
        {
            HideMask();
        }
        GetComponent<Button>().onClick.AddListener(SetSkin);
    }

    private void SetSkin()
    {
        if (IsUnlocked)
        {
            SkinManager.Instance.SetSkin(SkinImage.sprite);
        }
    }
    
    public SkinButton UnlockSkin()
    {
        IsUnlocked = true;
        HideMask();
        return this;
    }

    private void HideMask()
    {
        _mask.SetActive(false);
    }
}

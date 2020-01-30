using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Services;
using UnityEngine;
using UnityEngine.UI;

public class SkinButton : MonoBehaviour
{
    public Image SkinImage;
    [field: SerializeField]
    public bool IsUnlocked { get; private set; }

    [SerializeField] private GameObject _mask;
    private SkinService _skinService;

    public void Awake()
    {
        _skinService = ServiceLocator.GetService<SkinService>();

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
            _skinService.SetSkin(SkinImage.sprite);
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

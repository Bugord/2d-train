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
    [SerializeField] private GameObject _light;
    private SkinService _skinService;

    private Animator _animator;
    private readonly int _skinUnlocked = Animator.StringToHash("SkinUnlocked");

    public void Awake()
    {
        _skinService = ServiceLocator.GetService<SkinService>();
        _animator = _mask.GetComponent<Animator>();

        if (IsUnlocked)
        {
            HideMask(true);
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

    private void HideMask(bool onAwake = false)
    {
        SkinImage.color = Color.white;
        _light.SetActive(true);
        if (onAwake)
        {
            _mask.SetActive(false);
        }
        else
        {
            if (_animator != null) _animator.SetBool(_skinUnlocked, true);
        }
    }
}

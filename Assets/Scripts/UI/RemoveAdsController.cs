using Assets.Scripts.Managers;
using Assets.Scripts.Services;
using UnityEngine;
using UnityEngine.UI;

public class RemoveAdsController : MonoBehaviour
{
    private static Button _removeAdsButton;

    private IAPService _iapService;

    private void Awake()
    {
        _iapService = ServiceLocator.GetService<IAPService>();
        _removeAdsButton = GetComponent<Button>();
        _removeAdsButton.onClick.AddListener(RemoveAds);        
    }

    private void RemoveAds()
    {
        _iapService.BuyRemoveAds();
    }

    public static void DestroyButton()
    {
        if (_removeAdsButton != null)
        {
            Destroy(_removeAdsButton.gameObject);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class RemoveAdsController : MonoBehaviour
{
    private static Button _removeAdsButton;

    private void Awake()
    {
        _removeAdsButton = GetComponent<Button>();
        _removeAdsButton.onClick.AddListener(RemoveAds);        
    }

    private void RemoveAds()
    {
        ServiceLocator.GetService<IAPService>().BuyRemoveAds();
    }

    public static void DestroyButton()
    {
        if (_removeAdsButton != null)
        {
            Destroy(_removeAdsButton.gameObject);
        }
    }
}

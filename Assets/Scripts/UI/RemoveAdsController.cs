using System.Collections;
using System.Collections.Generic;
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
        IAPManager.Instance.BuyRemoveAds();
    }

    public static void DestroyButton()
    {
        Destroy(_removeAdsButton.gameObject);
    }
}

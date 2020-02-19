using Assets.Scripts.Services;
using Services;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class RemoveAdsController : MonoBehaviour
    {
        private static Button _removeAdsButton;
        private static AdsService _adsService;
        [SerializeField] private Text _text;
        
        private void Awake()
        {
            _removeAdsButton = GetComponent<Button>();
            _adsService = ServiceLocator.GetService<AdsService>();
            
            if (CloudVariables.IsAdsRemoved())
            {
                DestroyButton();
                return;
            }

            // if (IAPManager.Instance.AreAdsRemoved())
            // {
            //     DestroyButton();
            //     return;
            // }
            
            _removeAdsButton.onClick.AddListener(RemoveAds);
            if (_text != null)
            {
                _text.text = IAPManager.Instance.GetRemoveAdsPrice();   
            }
        }

        private void RemoveAds()
        {
            IAPManager.Instance.BuyRemoveAds();
        }

        public static void DestroyButton()
        {
            if (_removeAdsButton != null)
            {
                Destroy(_removeAdsButton.gameObject);
                _adsService.HideBanner();
            }
        }
    }
}

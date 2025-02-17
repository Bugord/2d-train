﻿using System;
using Assets.Scripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.Advertisements;

namespace Assets.Scripts.Services
{
    public class AdsService : IUnityAdsListener
    {
        public event Action TrainRevive;
        public event Action BonusCoins;

        public event Action<bool> ReviveAdvertisementUpdate;
        public event Action<bool> BonusAdvertisementUpdate;

        public event Action<int> FreeCoins;

        private AdsConfig _adsConfig;
        
        private DateTime lastGameOverTime {
            get
            {
                if (!PlayerPrefs.HasKey("LastGameOver"))
                {
                    return DateTime.MinValue;
                }
                
                return DateTime.Parse(PlayerPrefs.GetString("LastGameOver"));
            }
            set
            {
                PlayerPrefs.SetString("LastGameOver", value.ToString());
            }
        }

        private UIService _uiService;
        
        public AdsService(AdsConfig adsConfig)
        {
            _uiService = ServiceLocator.GetService<UIService>();
            
            _adsConfig = adsConfig;

            ReviveAdvertisementUpdate?.Invoke(Advertisement.IsReady(_adsConfig.ReviveVideoPlacementId));
            BonusAdvertisementUpdate?.Invoke(Advertisement.IsReady(_adsConfig.BonusVideoPlacementId));

            Advertisement.AddListener(this);
            Advertisement.Initialize(_adsConfig.GameId, _adsConfig.TestMode);
            Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        }
    
        public void ShowGameOverAdvertisement()
        {
            if ((float)(DateTime.Now - lastGameOverTime).TotalSeconds >= _adsConfig.GameOverPlacementTimeout)
            {
                Advertisement.Show(_adsConfig.GameOverPlacementId);
            }
        }

        public void ShowReviveVideoAdvertisement()
        {
            Advertisement.Show(_adsConfig.ReviveVideoPlacementId);
        }

        public void ShowBonusVideoAdvertisement()
        {
            Advertisement.Show(_adsConfig.BonusVideoPlacementId);
        }

        public void ShowFreeCoinsVideoAdvertisement()
        {
            Advertisement.Show(_adsConfig.FreeCoinsPlacementId);
        }

        public void ShowBanner()
        {
            Advertisement.Banner.Show(_adsConfig.BannerPlacementId);
        }

        public void HideBanner(bool destroy = false)
        {
            if (Advertisement.Banner.isLoaded)
            {
                Advertisement.Banner.Hide(destroy);
            }
        }

        public void OnUnityAdsReady(string placementId)
        {
            if (placementId == _adsConfig.ReviveVideoPlacementId)
            {
                ReviveAdvertisementUpdate?.Invoke(true);
            }
            else if (placementId == _adsConfig.BonusVideoPlacementId)
            {
                BonusAdvertisementUpdate?.Invoke(true);
            }
            else if (placementId == _adsConfig.BannerPlacementId && !CloudVariables.IsAdsRemoved() && _uiService.CurrentPanel is MainMenuController)
            {
                Advertisement.Banner.Show(_adsConfig.BannerPlacementId);
            }
        }

        public void OnUnityAdsDidError(string message)
        {
            throw new System.NotImplementedException();
        }

        public void OnUnityAdsDidStart(string placementId)
        {
            throw new System.NotImplementedException();
        }

        public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
        {
            // Define conditional logic for each ad completion status:
            if (showResult == ShowResult.Finished)
            {
                Debug.Log("Take your prize");
                if (placementId == _adsConfig.ReviveVideoPlacementId)
                {
                    TrainRevive?.Invoke();
                }

                if (placementId == _adsConfig.BonusVideoPlacementId)
                {
                    BonusCoins?.Invoke();
                }

                if (placementId == _adsConfig.FreeCoinsPlacementId)
                {
                    FreeCoins?.Invoke(100);
                }
            }
            else if (showResult == ShowResult.Skipped)
            {
                Debug.Log("Skipped");
            }
            else if (showResult == ShowResult.Failed)
            {
                Debug.LogWarning("The ad did not finish due to an error.");
            }

            if (showResult == ShowResult.Finished || showResult == ShowResult.Skipped)
            {
                lastGameOverTime = DateTime.Now;
            }
        }
    }
}

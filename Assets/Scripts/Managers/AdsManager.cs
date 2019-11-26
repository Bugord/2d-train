using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class AdsManager : MonoBehaviour, IUnityAdsListener
{
    private const string GAME_ID = "3364905";
    private const string GAME_OVER_PLACEMENT_ID = "GameOver";
    private const string REVIVE_VIDEO_PLACEMENT_ID = "ReviveVideo";
    private const string BONUS_VIDEO_PLACEMENT_ID = "BonusVideo";

    [SerializeField] private Button _testADS;
    [SerializeField] private Button _reviveButton;
    [SerializeField] private Button _bonusButton;

    [SerializeField]private bool _testMode = true;
    // Start is called before the first frame update

    public static event Action TrainRevive;
    public static event Action BonusCoins;

    void Start()
    {
        _testADS.interactable = Advertisement.IsReady(GAME_OVER_PLACEMENT_ID);
        _reviveButton.interactable = Advertisement.IsReady(REVIVE_VIDEO_PLACEMENT_ID);
        _bonusButton.interactable = Advertisement.IsReady(BONUS_VIDEO_PLACEMENT_ID);

        if (_testADS)
        {
            _testADS.onClick.AddListener(ShowGameOverAdvertisement);
        }

        if (_reviveButton)
        {
            _reviveButton.onClick.AddListener(ShowReviveVideoAdvertisement);
        }

        if (_bonusButton)
        {
            _bonusButton.onClick.AddListener(ShowBonusVideoAdvertisement);
        }

        Advertisement.AddListener(this);
        Advertisement.Initialize(GAME_ID, _testMode);
    }
    
    public void ShowGameOverAdvertisement()
    {
        Advertisement.Show(GAME_OVER_PLACEMENT_ID);
    }

    public void ShowReviveVideoAdvertisement()
    {
        Advertisement.Show(REVIVE_VIDEO_PLACEMENT_ID);
    }

    public void ShowBonusVideoAdvertisement()
    {
        Advertisement.Show(BONUS_VIDEO_PLACEMENT_ID);
    }

    public void OnUnityAdsReady(string placementId)
    {
        if (placementId == GAME_OVER_PLACEMENT_ID)
        {
            _testADS.interactable = true;
        }

        if (placementId == REVIVE_VIDEO_PLACEMENT_ID)
        {
            _reviveButton.interactable = true;
        }

        if (placementId == BONUS_VIDEO_PLACEMENT_ID)
        {
            _bonusButton.interactable = true;
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
            if (placementId == REVIVE_VIDEO_PLACEMENT_ID)
            {
                GameData.Revived = true;
                TrainRevive?.Invoke();
            }

            if (placementId == BONUS_VIDEO_PLACEMENT_ID)
            {
                GameData.BonusReceived = true;
                BonusCoins?.Invoke();
                _bonusButton.gameObject.SetActive(false);
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
    }
}

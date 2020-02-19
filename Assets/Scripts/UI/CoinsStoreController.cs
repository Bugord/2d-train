using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Services;
using Services;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CoinsStoreController : PanelBase
{
    [SerializeField] private Button _adsCoins;
    [SerializeField] private Button _coins2000;
    [SerializeField] private Button _coins5000;
    [SerializeField] private Button _coins12000;

    private UIService _uiService;
    private AdsService _adsService;
    private PlayGamesService _playGamesService;
    public event Action UpdateStoreCoins;
    
    private void Awake()
    {
        _uiService = ServiceLocator.GetService<UIService>();
        _adsService = ServiceLocator.GetService<AdsService>();
        _playGamesService = ServiceLocator.GetService<PlayGamesService>();

        _adsService.FreeCoins += AddCoins;
        IAPManager.Instance.CoinsPurchased += AddCoins;
        
        _adsCoins.onClick.AddListener(GetCoinsForAds);
        _coins2000.onClick.AddListener(() => { BuyCoins(2000); });
        _coins5000.onClick.AddListener(() => { BuyCoins(5000); });
        _coins12000.onClick.AddListener(() => { BuyCoins(12000); });
        
        _coins2000.GetComponentInChildren<Text>().text = IAPManager.Instance.GetCoinsPrice(2000);
        _coins5000.GetComponentInChildren<Text>().text = IAPManager.Instance.GetCoinsPrice(5000);
        _coins12000.GetComponentInChildren<Text>().text = IAPManager.Instance.GetCoinsPrice(12000);
    }

    private void BuyCoins(int coins)
    {
        IAPManager.Instance.BuyCoins(coins);
    }

    private void GetCoinsForAds()
    {
        _adsService.ShowFreeCoinsVideoAdvertisement();
    }

    private void AddCoins(int coins)
    {
        CloudVariables.ImportantValues[1] += coins;
        _playGamesService.SaveData();
        _uiService.UpdateMainMenu();
        UpdateStoreCoins?.Invoke();
    }
}

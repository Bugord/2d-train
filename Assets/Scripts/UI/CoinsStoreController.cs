using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Services;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CoinsStoreController : PanelBase
{
    public Button BackButton;

    [SerializeField] private Button _coins1000;
    [SerializeField] private Button _coins3000;
    [SerializeField] private Button _coins5000;
    [SerializeField] private Button _coins7000;
    [SerializeField] private Button _coins10000;

    private IAPService _iapService;

    private void Awake()
    {
        _iapService = ServiceLocator.GetService<IAPService>();
    }

    // Start is called before the first frame update
    void Start()
    {
        BackButton.onClick.AddListener(GoBack);

        _coins1000.onClick.AddListener(BuyCoins(1000));
        _coins3000.onClick.AddListener(BuyCoins(3000));
        _coins5000.onClick.AddListener(BuyCoins(5000));
        _coins7000.onClick.AddListener(BuyCoins(7000));
        _coins10000.onClick.AddListener(BuyCoins(10000));
    }
    private UnityAction BuyCoins(int coins)
    {
        return () => { _iapService.BuyCoins(coins); };
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinsStoreController : PanelBase
{
    public Button BackButton;

    [SerializeField] private Button _coins1000;
    [SerializeField] private Button _coins3000;
    [SerializeField] private Button _coins5000;
    [SerializeField] private Button _coins7000;
    [SerializeField] private Button _coins10000;

    // Start is called before the first frame update
    void Start()
    {
        BackButton.onClick.AddListener(GoBack);

        _coins1000.onClick.AddListener(Buy1000Coins);
        _coins3000.onClick.AddListener(Buy3000Coins);
        _coins5000.onClick.AddListener(Buy5000Coins);
        _coins7000.onClick.AddListener(Buy7000Coins);
        _coins10000.onClick.AddListener(Buy10000Coins);
    }
    private void Buy1000Coins()
    {
        IAPManager.Instance.BuyCoins(1000);
    }

    private void Buy3000Coins()
    {
        IAPManager.Instance.BuyCoins(3000);
    }

    private void Buy5000Coins()
    {
        IAPManager.Instance.BuyCoins(5000);
    }

    private void Buy7000Coins()
    {
        IAPManager.Instance.BuyCoins(7000);
    }

    private void Buy10000Coins()
    {
        IAPManager.Instance.BuyCoins(10000);
    }
}

using System;
using Assets.Scripts;
using Assets.Scripts.Services;
using UI;
using UnityEngine;
using UnityEngine.Purchasing;

// Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
    namespace Services
    {
        public class IAPManager : Singleton<IAPManager>, IStoreListener
        {
            private static IStoreController m_StoreController;          // The Unity Purchasing system.
            private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.
    
            public static string PRODUCT_REMOVE_ADS = "remove_ads";
            public static string PRODUCT_2000_COINS = "swipy_rails_coins_1000x";
            public static string PRODUCT_5000_COINS = "swipy_rails_coins_5000";
            public static string PRODUCT_12000_COINS = "swipy_rails_coins_7000";
            public event Action<int> CoinsPurchased; 
            
            private PlayGamesService _playGamesService;
            
            public void Start()
            {
                _playGamesService = ServiceLocator.GetService<PlayGamesService>();
                // If we haven't set up the Unity Purchasing reference
                if (m_StoreController == null)
                {
                    // Begin to configure our connection to Purchasing
                    InitializePurchasing();
                }
            }

            public void InitializePurchasing()
            {
                if (IsInitialized())
                {
                    return;
                }
                
                var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        
                builder.AddProduct(PRODUCT_REMOVE_ADS, ProductType.NonConsumable);
                builder.AddProduct(PRODUCT_2000_COINS, ProductType.Consumable);
                builder.AddProduct(PRODUCT_5000_COINS, ProductType.Consumable);
                builder.AddProduct(PRODUCT_12000_COINS, ProductType.Consumable);
                
                UnityPurchasing.Initialize(this, builder);
            }


            private bool IsInitialized()
            {
                // Only say we are initialized if both the Purchasing references are set.
                return m_StoreController != null && m_StoreExtensionProvider != null;
            }


            public void BuyRemoveAds()
            {
                // Buy the consumable product using its general identifier. Expect a response either 
                // through ProcessPurchase or OnPurchaseFailed asynchronously.
                BuyProductID(PRODUCT_REMOVE_ADS);
            }

            public void BuyCoins(int coins)
            {
                switch (coins)
                {
                    case 2000:
                        BuyProductID(PRODUCT_2000_COINS);
                        break;
                    case 5000:
                        BuyProductID(PRODUCT_5000_COINS);
                        break;
                    case 12000:
                        BuyProductID(PRODUCT_12000_COINS);
                        break;
                }
            }

            private void BuyProductID(string productId)
            {
                // If Purchasing has been initialized ...
                if (IsInitialized())
                {
                    // ... look up the Product reference with the general product identifier and the Purchasing 
                    // system's products collection.
                    Product product = m_StoreController.products.WithID(productId);

                    // If the look up found a product for this device's store and that product is ready to be sold ... 
                    if (product != null && product.availableToPurchase)
                    {
                        Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                        // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                        // asynchronously.
                        m_StoreController.InitiatePurchase(product);
                    }
                    // Otherwise ...
                    else
                    {
                        // ... report the product look-up failure situation  
                        Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                    }
                }
                // Otherwise ...
                else
                {
                    // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
                    // retrying initiailization.
                    Debug.Log("BuyProductID FAIL. Not initialized.");
                }
            }
    
            public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
            {
                // Purchasing has succeeded initializing. Collect our Purchasing references.
                Debug.Log("OnInitialized: PASS");
                
                // Overall Purchasing system, configured with products for this application.
                m_StoreController = controller;
                // Store specific subsystem, for accessing device-specific store features.
                m_StoreExtensionProvider = extensions;
            }
            public void OnInitializeFailed(InitializationFailureReason error)
            {
                // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
                Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
            }
    
            public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
            {
                // A consumable product has been purchased by this user.
                if (String.Equals(args.purchasedProduct.definition.id, PRODUCT_REMOVE_ADS, StringComparison.Ordinal))
                {
                    CloudVariables.ImportantValues[2] = 1;
                    RemoveAdsController.DestroyButton();
                } 
                else if (String.Equals(args.purchasedProduct.definition.id, PRODUCT_2000_COINS, StringComparison.Ordinal))
                {
                    CoinsPurchased?.Invoke(2000);
                }
                else if (String.Equals(args.purchasedProduct.definition.id, PRODUCT_5000_COINS, StringComparison.Ordinal))
                {
                    CoinsPurchased?.Invoke(5000);
                }
                else if (String.Equals(args.purchasedProduct.definition.id, PRODUCT_12000_COINS, StringComparison.Ordinal))
                {
                    CoinsPurchased?.Invoke(12000);
                }
                else
                {
                    Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
                }

                _playGamesService.SaveData();
                
                // Return a flag indicating whether this product has completely been received, or if the application needs 
                // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
                // saving purchased products to the cloud, and when that save is delayed. 
                return PurchaseProcessingResult.Complete;
            }


            public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
            {
                // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
                // this reason with the user to guide their troubleshooting actions.
                Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
            }
        }
    }
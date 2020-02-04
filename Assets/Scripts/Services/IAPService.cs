﻿using System;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.Purchasing;

// Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
    namespace Assets.Scripts.Services
    {
        public class IAPService : IStoreListener
        {
            private static IStoreController m_StoreController;          // The Unity Purchasing system.
            private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.
    
            public static string PRODUCT_REMOVE_ADS = "remove_ads";
            public static string PRODUCT_1000_COINS = "swipy_rails_coins_1000x";
            public static string PRODUCT_3000_COINS = "swipy_rails_coins_3000";
            public static string PRODUCT_5000_COINS = "swipy_rails_coins_5000";
            public static string PRODUCT_7000_COINS = "swipy_rails_coins_7000";
            public static string PRODUCT_10000_COINS = "swipy_rails_coins_10000";

            private PlayGamesService _playGamesService;
            private UIService _uiService;

            public IAPService()
            {
                _uiService = ServiceLocator.GetService<UIService>();
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
                builder.AddProduct(PRODUCT_1000_COINS, ProductType.Consumable);
                builder.AddProduct(PRODUCT_3000_COINS, ProductType.Consumable);
                builder.AddProduct(PRODUCT_5000_COINS, ProductType.Consumable);
                builder.AddProduct(PRODUCT_7000_COINS, ProductType.Consumable);
                builder.AddProduct(PRODUCT_10000_COINS, ProductType.Consumable);
        
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
                    case 1000:
                        BuyProductID(PRODUCT_1000_COINS);
                        break;
                    case 3000:
                        BuyProductID(PRODUCT_3000_COINS);
                        break;
                    case 5000:
                        BuyProductID(PRODUCT_5000_COINS);
                        break;
                    case 7000:
                        BuyProductID(PRODUCT_7000_COINS);
                        break;
                    case 10000:
                        BuyProductID(PRODUCT_10000_COINS);
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
                    Debug.LogError("Ads removed." + args.purchasedProduct.metadata.localizedDescription);
                    CloudVariables.ImportantValues[2] = 1;
                    RemoveAdsController.DestroyButton();
                } 
                else if (String.Equals(args.purchasedProduct.definition.id, PRODUCT_1000_COINS, StringComparison.Ordinal))
                {
                    Debug.LogError("1000 Coins." + args.purchasedProduct.metadata.localizedDescription);
                    CloudVariables.ImportantValues[1] += 1000;
                }
                else if (String.Equals(args.purchasedProduct.definition.id, PRODUCT_3000_COINS, StringComparison.Ordinal))
                {
                    Debug.LogError("3000 Coins." + args.purchasedProduct.metadata.localizedDescription);
                    CloudVariables.ImportantValues[1] += 3000;
                }
                else if (String.Equals(args.purchasedProduct.definition.id, PRODUCT_5000_COINS, StringComparison.Ordinal))
                {
                    Debug.LogError("5000 Coins." + args.purchasedProduct.metadata.localizedDescription);
                    CloudVariables.ImportantValues[1] += 5000;
                }
                else if (String.Equals(args.purchasedProduct.definition.id, PRODUCT_7000_COINS, StringComparison.Ordinal))
                {
                    Debug.LogError("7000 Coins." + args.purchasedProduct.metadata.localizedDescription);
                    CloudVariables.ImportantValues[1] += 7000;
                }
                else if (String.Equals(args.purchasedProduct.definition.id, PRODUCT_10000_COINS, StringComparison.Ordinal))
                {
                    Debug.LogError("10000 Coins." + args.purchasedProduct.metadata.localizedDescription);
                    CloudVariables.ImportantValues[1] += 10000;
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
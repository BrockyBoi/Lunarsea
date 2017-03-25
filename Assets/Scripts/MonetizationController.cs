using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Purchasing;
//using UnityEngine.Advertisements;

public class MonetizationController : MonoBehaviour, IStoreListener
{

    public static MonetizationController controller;
    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

    public static string PRODUCT_1000_COINS = "coins1000";
    public static string PRODUCT_5000_COINS = "coins5000";
    public static string PRODUCT_20000_COINS = "coins20000";
    public static string PRODUCT_100000_COINS = "coins100000";
    public static string PRODUCT_1000000_COINS = "coins1000000";
    public static string PRODUCT_NO_ADS = "noads";

    bool adsTurnedOff;

    void Start()
    {

        if (m_StoreController == null)
        {
            InitializePurchasing();
        }
    }

    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            return;
        }

        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());


        builder.AddProduct(PRODUCT_1000_COINS, ProductType.Consumable);
        builder.AddProduct(PRODUCT_5000_COINS, ProductType.Consumable);
        builder.AddProduct(PRODUCT_20000_COINS, ProductType.Consumable);
        builder.AddProduct(PRODUCT_100000_COINS, ProductType.Consumable);
        builder.AddProduct(PRODUCT_1000000_COINS, ProductType.Consumable);

        builder.AddProduct(PRODUCT_NO_ADS, ProductType.NonConsumable);

        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);
    }


    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }


    public void Buy1000Coins()
    {
        BuyProductID(PRODUCT_1000_COINS);
    }
    public void Buy5000Coins()
    {
        BuyProductID(PRODUCT_5000_COINS);
    }
    public void Buy20000Coins()
    {
        BuyProductID(PRODUCT_20000_COINS);
    }
    public void Buy100000Coins()
    {
        BuyProductID(PRODUCT_100000_COINS);
    }
    public void Buy1000000Coins()
    {
        BuyProductID(PRODUCT_1000000_COINS);
    }
    public void BuyNoAds()
    {
        BuyProductID(PRODUCT_NO_ADS);
    }





    void BuyProductID(string productId)
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


    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) =>
            {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }


    //  
    // --- IStoreListener
    //

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
        if (String.Equals(args.purchasedProduct.definition.id, PRODUCT_1000_COINS, StringComparison.Ordinal))
        {
            Debug.Log("Just bought 1000 gold");
            CoinController.controller.BuyCoins(1000);
        }
        // Or ... a non-consumable product has been purchased by this user.
        else if (String.Equals(args.purchasedProduct.definition.id, PRODUCT_5000_COINS, StringComparison.Ordinal))
        {
            Debug.Log("Just bought 5000 gold");
            CoinController.controller.BuyCoins(5000);
        }
        // Or ... a subscription product has been purchased by this user.
        else if (String.Equals(args.purchasedProduct.definition.id, PRODUCT_20000_COINS, StringComparison.Ordinal))
        {
            Debug.Log("Just bought 20000 gold");
            CoinController.controller.BuyCoins(20000);
        }
        else if (String.Equals(args.purchasedProduct.definition.id, PRODUCT_100000_COINS, StringComparison.Ordinal))
        {
            Debug.Log("Just bought 100000 gold");
            CoinController.controller.BuyCoins(100000);
        }
        else if (String.Equals(args.purchasedProduct.definition.id, PRODUCT_1000000_COINS, StringComparison.Ordinal))
        {
            Debug.Log("Just bought 1000000 gold");
            CoinController.controller.BuyCoins(1000000);
        }
        else if (String.Equals(args.purchasedProduct.definition.id, PRODUCT_NO_ADS, StringComparison.Ordinal))
        {
            Debug.Log("Just turned off ads");
            adsTurnedOff = true;
        }
        // Or ... an unknown product has been purchased by this user. Fill in additional products here....
        else
        {
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
        }

        PlayerInfo.controller.Save();

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





    // UNITY ADS




    [SerializeField] string gameID = "33675";

    void Awake()
    {
        controller = this;
        //Advertisement.Initialize(gameID, true);
    }

    public void UpdateAdsTurnedOff(bool b)
    {
        adsTurnedOff = b;
    }

    public bool CheckIfAdsTurnedOff()
    {
        return adsTurnedOff;
    }

    public void ShowNormalAd()
    {
        // if (Advertisement.IsReady())
        //     Advertisement.Show();
    }

    public void ShowAd(string zone = "")
    {
// #if UNITY_EDITOR
//         StartCoroutine(WaitForAd());
// #endif

//         if (string.Equals(zone, ""))
//             zone = null;

//         ShowOptions options = new ShowOptions();
//         options.resultCallback = AdCallbackhandler;

//         if (Advertisement.IsReady(zone))
//             Advertisement.Show(zone, options);
    }

    // void AdCallbackhandler(ShowResult result)
    // {
    //     switch (result)
    //     {
    //         case ShowResult.Finished:
    //             Debug.Log("Ad Finished. Rewarding player...");
    //             CoinController.controller.BuyCoins(200);
    //             break;
    //         case ShowResult.Skipped:
    //             Debug.Log("Ad skipped. Son, I am dissapointed in you");
    //             break;
    //         case ShowResult.Failed:
    //             Debug.Log("I swear this has never happened to me before");
    //             break;
    //     }
    // }

    // IEnumerator WaitForAd()
    // {
    //     float currentTimeScale = Time.timeScale;
    //     Time.timeScale = 0f;
    //     yield return null;

    //     while (Advertisement.isShowing)
    //         yield return null;

    //     Time.timeScale = currentTimeScale;
    // }

}

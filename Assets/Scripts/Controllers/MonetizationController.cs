using System;
using UnityEngine;
using UnityEngine.Purchasing;
using GoogleMobileAds;
using GoogleMobileAds.Api;

public class MonetizationController : MonoBehaviour, IStoreListener
{
	#region IAP

	public static MonetizationController controller;
	private static IStoreController m_StoreController;
	// The Unity Purchasing system.
	private static IExtensionProvider m_StoreExtensionProvider;
	// The store-specific Purchasing subsystems.

	delegate void GoalMultDel (float num);

	GoalMultDel GoalMultiplier;

	public static string PRODUCT_1000_COINS = "coins1000";
	public static string PRODUCT_5000_COINS = "coins5000";
	public static string PRODUCT_20000_COINS = "coins20000";
	public static string PRODUCT_100000_COINS = "coins100000";
	public static string PRODUCT_1000000_COINS = "coins1000000";
	public static string PRODUCT_NO_ADS = "noads";

	void Start ()
	{
		GoalMultiplier = TempGoalController.controller.SetScoreMultiplier;
		if (m_StoreController == null) {
			InitializePurchasing ();
		}

		#if UNITY_IOS || UNITY_ANDROID
		rewardAd = RewardBasedVideoAd.Instance;

		rewardAd.OnAdClosed += HandleOnAdClosed;
		rewardAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;
		rewardAd.OnAdLeavingApplication += HandleOnLeavingApplication;
		rewardAd.OnAdLoaded += HandleOnAdLoaded;
		rewardAd.OnAdOpening += HandleOnAdOpening;
		rewardAd.OnAdRewarded += HandleOnAdRewarded;
		rewardAd.OnAdStarted += HandleOnAdStarted;

		GenerateNormalAd ();
		GenerateRewardAd ();
		#endif
	}

	public void InitializePurchasing ()
	{
		// If we have already connected to Purchasing ...
		if (IsInitialized ()) {
			// ... we are done here.
			return;
		}

		// Create a builder, first passing in a suite of Unity provided stores.
		var builder = ConfigurationBuilder.Instance (StandardPurchasingModule.Instance ());


		builder.AddProduct (PRODUCT_1000_COINS, ProductType.Consumable);
		builder.AddProduct (PRODUCT_5000_COINS, ProductType.Consumable);
		builder.AddProduct (PRODUCT_20000_COINS, ProductType.Consumable);
		builder.AddProduct (PRODUCT_100000_COINS, ProductType.Consumable);
		builder.AddProduct (PRODUCT_1000000_COINS, ProductType.Consumable);

		builder.AddProduct (PRODUCT_NO_ADS, ProductType.NonConsumable);

		// Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
		// and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
		UnityPurchasing.Initialize (this, builder);
	}

	void OnDisable ()
	{
		#if UNITY_ANDROID || UNITY_IOS
		rewardAd.OnAdClosed -= HandleOnAdClosed;
		rewardAd.OnAdFailedToLoad -= HandleOnAdFailedToLoad;
		rewardAd.OnAdLeavingApplication -= HandleOnLeavingApplication;
		rewardAd.OnAdLoaded -= HandleOnAdLoaded;
		rewardAd.OnAdOpening -= HandleOnAdOpening;
		rewardAd.OnAdRewarded -= HandleOnAdRewarded;
		rewardAd.OnAdStarted -= HandleOnAdStarted;

		if (interstitialAd != null) {
			interstitialAd.OnAdClosed -= HandleOnAdClosed;
			interstitialAd.OnAdFailedToLoad -= HandleOnAdFailedToLoad;
			interstitialAd.OnAdLeavingApplication -= HandleOnLeavingApplication;
			interstitialAd.OnAdLoaded -= HandleOnAdLoaded;
			interstitialAd.OnAdOpening -= HandleOnAdOpening;
			interstitialAd.Destroy ();
		}
		#endif
	}


	private bool IsInitialized ()
	{
		// Only say we are initialized if both the Purchasing references are set.
		return m_StoreController != null && m_StoreExtensionProvider != null;
	}


	public void Buy1000Coins ()
	{
		BuyProductID (PRODUCT_1000_COINS);

	}

	public void Buy5000Coins ()
	{
		BuyProductID (PRODUCT_5000_COINS);

	}

	public void Buy20000Coins ()
	{
		BuyProductID (PRODUCT_20000_COINS);

	}

	public void Buy100000Coins ()
	{
		BuyProductID (PRODUCT_100000_COINS);

	}

	public void Buy1000000Coins ()
	{
		BuyProductID (PRODUCT_1000000_COINS);

	}

	public void BuyNoAds ()
	{
		BuyProductID (PRODUCT_NO_ADS);
	}


	void BuyProductID (string productId)
	{
		// If Purchasing has been initialized ...
		if (IsInitialized ()) {
			// ... look up the Product reference with the general product identifier and the Purchasing 
			// system's products collection.
			Product product = m_StoreController.products.WithID (productId);

			// If the look up found a product for this device's store and that product is ready to be sold ... 
			if (product != null && product.availableToPurchase) {
				Debug.Log (string.Format ("Purchasing product asychronously: '{0}'", product.definition.id));
				// ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
				// asynchronously.
				m_StoreController.InitiatePurchase (product);
			}
            // Otherwise ...
            else {
				// ... report the product look-up failure situation  
				Debug.Log ("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
			}
		}
        // Otherwise ...
        else {
			// ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
			// retrying initiailization.
			Debug.Log ("BuyProductID FAIL. Not initialized.");
		}
	}


	// Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google.
	// Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
	public void RestorePurchases ()
	{
		// If Purchasing has not yet been set up ...
		if (!IsInitialized ()) {
			// ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
			Debug.Log ("RestorePurchases FAIL. Not initialized.");
			return;
		}

		// If we are running on an Apple device ... 
		if (Application.platform == RuntimePlatform.IPhonePlayer ||
		    Application.platform == RuntimePlatform.OSXPlayer) {
			// ... begin restoring purchases
			Debug.Log ("RestorePurchases started ...");

			// Fetch the Apple store-specific subsystem.
			var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions> ();
			// Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
			// the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
			apple.RestoreTransactions ((result) => {
				// The first phase of restoration. If no more responses are received on ProcessPurchase then 
				// no purchases are available to be restored.
				Debug.Log ("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
			});
		}
        // Otherwise ...
        else {
			// We are not running on an Apple device. No work is necessary to restore purchases.
			Debug.Log ("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
		}
	}


	//
	// --- IStoreListener
	//

	public void OnInitialized (IStoreController controller, IExtensionProvider extensions)
	{
		// Purchasing has succeeded initializing. Collect our Purchasing references.
//        Debug.Log("OnInitialized: PASS");

		// Overall Purchasing system, configured with products for this application.
		m_StoreController = controller;
		// Store specific subsystem, for accessing device-specific store features.
		m_StoreExtensionProvider = extensions;
	}


	public void OnInitializeFailed (InitializationFailureReason error)
	{
		// Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
		Debug.Log ("OnInitializeFailed InitializationFailureReason:" + error);
	}


	public PurchaseProcessingResult ProcessPurchase (PurchaseEventArgs args)
	{

		// A consumable product has been purchased by this user.
		if (String.Equals (args.purchasedProduct.definition.id, PRODUCT_1000_COINS, StringComparison.Ordinal)) {
			CoinController.controller.BuyCoins (1000);
			GoalMultiplier (1.25f);
			AudioController.controller.PlayFX (AudioController.controller.coinPurchase);
		}
        // Or ... a non-consumable product has been purchased by this user.
        else if (String.Equals (args.purchasedProduct.definition.id, PRODUCT_5000_COINS, StringComparison.Ordinal)) {
			CoinController.controller.BuyCoins (5000);
			GoalMultiplier (1.5f);
			AudioController.controller.PlayFX (AudioController.controller.coinPurchase);
		}
        // Or ... a subscription product has been purchased by this user.
        else if (String.Equals (args.purchasedProduct.definition.id, PRODUCT_20000_COINS, StringComparison.Ordinal)) {
			CoinController.controller.BuyCoins (20000);
			GoalMultiplier (2f);
			AudioController.controller.PlayFX (AudioController.controller.coinPurchase);
		} else if (String.Equals (args.purchasedProduct.definition.id, PRODUCT_100000_COINS, StringComparison.Ordinal)) {
			CoinController.controller.BuyCoins (100000);
			GoalMultiplier (2.5f);
			AudioController.controller.PlayFX (AudioController.controller.coinPurchase);
		} else if (String.Equals (args.purchasedProduct.definition.id, PRODUCT_1000000_COINS, StringComparison.Ordinal)) {
			CoinController.controller.BuyCoins (1000000);
			GoalMultiplier (5f);
			AudioController.controller.PlayFX (AudioController.controller.coinPurchase);
		} else if (String.Equals (args.purchasedProduct.definition.id, PRODUCT_NO_ADS, StringComparison.Ordinal)) {
			adsTurnedOff = true;
		}
        // Or ... an unknown product has been purchased by this user. Fill in additional products here....
        else {
			Debug.Log (string.Format ("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
		}

		PlayerInfo.controller.Save ();

		// Return a flag indicating whether this product has completely been received, or if the application needs 
		// to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
		// saving purchased products to the cloud, and when that save is delayed. 
		return PurchaseProcessingResult.Complete;
	}


	public void OnPurchaseFailed (Product product, PurchaseFailureReason failureReason)
	{
		// A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
		// this reason with the user to guide their troubleshooting actions.
		Debug.Log (string.Format ("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
	}

	#endregion

	#region Ads

	[SerializeField]
	string androidVideoID;
	[SerializeField]
	string androidRewardVideoID;

	[SerializeField]
	string iosVideoID;
	[SerializeField]
	string iosRewardVideoID;

	bool adsTurnedOff;

	int rewardedAdsWatchedToday;


	RewardBasedVideoAd rewardAd;
	InterstitialAd interstitialAd;

	void Awake ()
	{
		controller = this;
		//Reward ads initialized in Start fucntion at top of script
	}

	void CreateNewInterstitialAd (string ID)
	{
		interstitialAd = new InterstitialAd (ID);

		interstitialAd.OnAdClosed += HandleOnAdClosed;
		interstitialAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;
		interstitialAd.OnAdLeavingApplication += HandleOnLeavingApplication;
		interstitialAd.OnAdLoaded += HandleOnAdLoaded;
		interstitialAd.OnAdOpening += HandleOnAdOpening;
	}

	void GenerateNormalAd ()
	{
		
		#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
		string adID = "unused";	
		#elif UNITY_ANDROID
		string adID = androidVideoID;
		#elif UNITY_IOS
		string adID = iosVideoID;
		#endif

		CreateNewInterstitialAd (adID);
		interstitialAd.LoadAd (new AdRequest.Builder ().Build ());

		//ShowNormalAd ();
	}

	public void ShowNormalAd ()
	{
		if (interstitialAd.IsLoaded ())
			interstitialAd.Show ();
	}

	void GenerateRewardAd ()
	{
		#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
		string adID = "unused";
		#elif UNITY_ANDROID
		string adID = androidRewardVideoID;
		#elif UNITY_IOS
		string adID = iosRewardVideoID;
		#endif
		rewardAd.LoadAd (new AdRequest.Builder ().Build (), adID);

		//ShowRewardAd ();
	}


	public void ShowRewardAd ()
	{
		if (rewardAd.IsLoaded ()) {
			rewardAd.Show ();
		} else {
			Debug.Log ("No reward ad ready");
		}
	}

	public bool CheckIfAdsTurnedOff ()
	{
		return adsTurnedOff;
	}

	public void SetAdsTurnedOff (bool b)
	{
		adsTurnedOff = b;
	}

	#region Handlers

	public void HandleOnAdLoaded (object sender, EventArgs args)
	{

	}



	public void HandleOnAdFailedToLoad (object sender, AdFailedToLoadEventArgs args)
	{
		//Reload
	}



	public void HandleOnAdOpening (object sender, EventArgs args)
	{
		//Pause game
	}



	public void HandleOnAdStarted (object sender, EventArgs args)
	{
		//Mute audio
		AudioController.controller.MuteAll ();
	}



	public void HandleOnAdClosed (object sender, EventArgs args)
	{
		//Restore audio
		AudioController.controller.RestoreSound ();

		if (interstitialAd != null) {
			interstitialAd.OnAdClosed -= HandleOnAdClosed;
			interstitialAd.OnAdFailedToLoad -= HandleOnAdFailedToLoad;
			interstitialAd.OnAdLeavingApplication -= HandleOnLeavingApplication;
			interstitialAd.OnAdLoaded -= HandleOnAdLoaded;
			interstitialAd.OnAdOpening -= HandleOnAdOpening;
			interstitialAd.Destroy ();

			GenerateNormalAd ();
		}

	}



	public void HandleOnAdRewarded (object sender, Reward args)
	{
		//Reward with 100 coins
		Debug.Log ("You just got " + args.Amount.ToString () + " " + args.Type.ToString () + "!");
		AudioController.controller.PlayFX (AudioController.controller.coinPurchase);
		CoinController.controller.ReceiveReward (150);
	}


	public void HandleOnLeavingApplication (object sender, EventArgs args)
	{

	}

	#endregion

	#endregion

	#region DailyRewards

	string stringDate;
	//http://answers.unity3d.com/questions/776823/daily-bonus.html
	DateTime oldDate;

	public void DayCheck ()
	{
		// DateTime newDate = System.DateTime.Now;

		// if (oldDate == null)
		//     oldDate = System.DateTime.Now;

		// TimeSpan diff = newDate.Subtract(oldDate);
		// if (diff.Days >= 1)
		// {
		//     rewardedAdsWatchedToday = 0;
		//     TempGoalController.controller.ResetTempMults();
		//     CoinController.controller.GiveDailyReward();
		//     oldDate = newDate;
		//     PlayerInfo.controller.Save();
		// }
	}

	public DateTime GetOldDate ()
	{
		return oldDate;

	}

	public void SetOldDate (DateTime dT)
	{
		oldDate = dT;
		DayCheck ();
	}

	#endregion
}

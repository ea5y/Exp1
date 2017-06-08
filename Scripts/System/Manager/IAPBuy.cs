#if UNITY_IOS
#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// You must obfuscate your secrets using Window > Unity IAP > Receipt Validation Obfuscator
// before receipt validation will compile in this sample.
//#define RECEIPT_VALIDATION
#endif

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.UI;

#if RECEIPT_VALIDATION
using UnityEngine.Purchasing.Security;
#endif

public class IAPBuy : MonoBehaviour, IStoreListener
{
	public static IAPBuy Instance;
	// Unity IAP objects
	private IStoreController m_Controller;
	private IAppleExtensions m_AppleExtensions;
	private IMoolahExtension m_MoolahExtensions;
	private ISamsungAppsExtensions m_SamsungExtensions;
	private IMicrosoftExtensions m_MicrosoftExtensions;

	#pragma warning disable 0414
	private bool m_IsGooglePlayStoreSelected;
	#pragma warning restore 0414
	private bool m_IsSamsungAppsStoreSelected;
	private bool m_IsCloudMoolahStoreSelected;

	private string m_LastTransationID;
	private string m_LastReceipt;
	private string m_CloudMoolahUserName;
	private bool m_IsLoggedIn = false;

	private int m_SelectedItemIndex = -1;
	// -1 == no product
	private bool m_PurchaseInProgress;
	private Selectable m_InteractableSelectable;
	// Optimization used for UI state management

	#if RECEIPT_VALIDATION
	private CrossPlatformValidator validator;
	#endif

	/// <summary>
	/// This will be called when Unity IAP has finished initialising.
	/// </summary>
	public void OnInitialized (IStoreController controller, IExtensionProvider extensions)
	{
		m_Controller = controller;
		m_AppleExtensions = extensions.GetExtension<IAppleExtensions> ();
//		m_SamsungExtensions = extensions.GetExtension<ISamsungAppsExtensions> ();
//		m_MoolahExtensions = extensions.GetExtension<IMoolahExtension> ();
//		m_MicrosoftExtensions = extensions.GetExtension<IMicrosoftExtensions> ();

		InitUI ();

		// On Apple platforms we need to handle deferred purchases caused by Apple's Ask to Buy feature.
		// On non-Apple platforms this will have no effect; OnDeferred will never be called.
		m_AppleExtensions.RegisterPurchaseDeferredListener (OnDeferred);
	}

	Action<string, string> ProcessPurchaseCallBack;

	public PurchaseProcessingResult ProcessPurchase (PurchaseEventArgs e)
	{
		Debug.Log ("Purchase OK: " + e.purchasedProduct.definition.id);
		Debug.Log ("Receipt: " + e.purchasedProduct.receipt);
//		Debug.Log ("Decode Receipt: " + System.Convert.FromBase64String(e.purchasedProduct.receipt));

		m_LastTransationID = e.purchasedProduct.transactionID;
		m_LastReceipt = e.purchasedProduct.receipt;
		if (null != ProcessPurchaseCallBack) {
			ProcessPurchaseCallBack (m_LastTransationID, m_LastReceipt);
		}
		TalkingDataMgr.Instance.Pay (PluginController.AuthInfo.authID, m_LastTransationID, GetCoinCost(m_LastSpend));
		m_PurchaseInProgress = false;


		#if RECEIPT_VALIDATION
		// Local validation is available for Apple stores
		{
			try {
				var result = validator.Validate (e.purchasedProduct.receipt);
				Debug.Log ("Receipt is valid. Contents:");
				foreach (IPurchaseReceipt productReceipt in result) {
					Debug.Log (productReceipt.productID);
					Debug.Log (productReceipt.purchaseDate);
					Debug.Log (productReceipt.transactionID);

					AppleInAppPurchaseReceipt apple = productReceipt as AppleInAppPurchaseReceipt;
					if (null != apple) {
						Debug.Log (apple.originalTransactionIdentifier);
						Debug.Log (apple.subscriptionExpirationDate);
						Debug.Log (apple.cancellationDate);
						Debug.Log (apple.quantity);
					}
				}
			} catch (IAPSecurityException) {
				Debug.Log ("Invalid receipt, not unlocking content");
				return PurchaseProcessingResult.Complete;
			}
		}
		#endif
		return PurchaseProcessingResult.Complete;
	}

	Action<string> PurchaseFailedCallback;

	public void OnPurchaseFailed (Product item, PurchaseFailureReason r)
	{
		Debug.Log ("Purchase failed: " + item.definition.id);
		Debug.Log (r);

		m_PurchaseInProgress = false;
		if (null != PurchaseFailedCallback) {
			PurchaseFailedCallback (r.ToString ());
		}
	}

	public void OnInitializeFailed (InitializationFailureReason error)
	{
		Debug.Log ("Billing failed to initialize!");
		switch (error) {
		case InitializationFailureReason.AppNotKnown:
			Debug.LogError ("Is your App correctly uploaded on the relevant publisher console?");
			break;
		case InitializationFailureReason.PurchasingUnavailable:
			// Ask the user if billing is disabled in device settings.
			Debug.Log ("Billing disabled!");
			break;
		case InitializationFailureReason.NoProductsAvailable:
			// Developer configuration error; check product metadata.
			Debug.Log ("No products available for purchase!");
			break;
		}
	}

	public void Awake ()
	{
		Instance = this;
		var module = StandardPurchasingModule.Instance ();
		module.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
		var builder = ConfigurationBuilder.Instance (module);
		m_IsGooglePlayStoreSelected = Application.platform == RuntimePlatform.Android && module.androidStore == AndroidStore.GooglePlay;
		m_IsCloudMoolahStoreSelected = Application.platform == RuntimePlatform.Android && module.androidStore == AndroidStore.CloudMoolah;

		// Define our products.
		builder.AddProduct ("10.coin", ProductType.Consumable, new IDs {
			{ "com.xworld.coin10", AppleAppStore.Name },
		});
		builder.AddProduct ("60.coin", ProductType.Consumable, new IDs {
			{ "com.xworld.coin60", AppleAppStore.Name },
		});
		builder.AddProduct ("425.coin", ProductType.Consumable, new IDs {
			{ "com.xworld.coin425", AppleAppStore.Name },
		});
		builder.AddProduct ("725.coin", ProductType.Consumable, new IDs {
			{ "com.xworld.coin725", AppleAppStore.Name },
		});
		builder.AddProduct ("1240.coin", ProductType.Consumable, new IDs {
			{ "com.xworld.coin1240", AppleAppStore.Name },
		});
		builder.AddProduct ("2100.coin", ProductType.Consumable, new IDs {
			{ "com.xworld.coin2100", AppleAppStore.Name },
		});
		builder.AddProduct ("3690.coin", ProductType.Consumable, new IDs {
			{ "com.xworld.coin3690", AppleAppStore.Name },
		});
		builder.AddProduct ("6868.coin", ProductType.Consumable, new IDs {
			{ "com.xworld.coin6868", AppleAppStore.Name },
		});
		m_IsSamsungAppsStoreSelected = Application.platform == RuntimePlatform.Android && module.androidStore == AndroidStore.SamsungApps;

		#if RECEIPT_VALIDATION
		string appIdentifier;
		#if UNITY_5_6_OR_NEWER
		appIdentifier = Application.identifier;
		#else
		appIdentifier = Application.bundleIdentifier;
		#endif
		validator = new CrossPlatformValidator (GooglePlayTangle.Data (), AppleTangle.Data (), appIdentifier);
		#endif

		// Now we're ready to initialize Unity IAP.
		UnityPurchasing.Initialize (this, builder);
	}

	/// <summary>
	/// This will be called after a call to IAppleExtensions.RestoreTransactions().
	/// </summary>
	private void OnTransactionsRestored (bool success)
	{
		Debug.Log ("Transactions restored.");
	}

	private void OnDeferred (Product item)
	{
		Debug.Log ("Purchase deferred: " + item.definition.id);
	}

	private void InitUI ()
	{
		// Show Restore button on supported platforms
		#if UNITY_IOS

		#endif
	}

	public void RegisterFailed (FastRegisterError error, string errorMessage)
	{
		Debug.Log ("RegisterFailed: error = " + error.ToString () + ", errorMessage = " + errorMessage);
	}

	private IosBuySpend m_LastSpend;
	public void Buy (IosBuySpend pSpend, string payload, Action<string, string> pSuccessCallback, Action<string> pFailedCallback)
	{
		if (m_PurchaseInProgress == true) {
			Debug.Log ("Please wait, purchasing ...");
			return;
		}
		ProcessPurchaseCallBack = pSuccessCallback;
		PurchaseFailedCallback = pFailedCallback;
		m_PurchaseInProgress = true;
		m_LastSpend = pSpend;
		m_Controller.InitiatePurchase (m_Controller.products.all [(int)pSpend], payload);
	}

	public void Restore ()
	{
		m_AppleExtensions.RestoreTransactions (OnTransactionsRestored);
	}

	public void BuyTest ()
	{
		//Buy (IosBuySpend.coin60);
	}

	public int GetCoinCost (IosBuySpend coin)
	{
		switch (coin) {
		case IosBuySpend.coin10:
			return 10;
		case IosBuySpend.coin60:
			return 60;
		case IosBuySpend.coin425:
			return 425;
		case IosBuySpend.coin725:
			return 725;
		case IosBuySpend.coin1245:
			return 1245;
		case IosBuySpend.coin2100:
			return 2100;
		case IosBuySpend.coin3690:
			return 3690;
		case IosBuySpend.coin6868:
			return 6868;
		default:
			return 0;
		}
	}
}
#endif

public enum IosBuySpend
{
	coin10 = 0,
	coin60,
	coin425,
	coin725,
	coin1245,
	coin2100,
	coin3690,
	coin6868,
}

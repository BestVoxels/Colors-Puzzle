using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

// Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
public class GameIAP : MonoBehaviour, IStoreListener
{
    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

    // We can have the same product identifier for both stores
    // unless we want to sell on iOS & Mac (we will need store ID specific for each platform)
    private static string[] _products =
    {
        "just_revive",
        "theme1",
        "theme2",
        "theme3",
        "theme4",
        "theme5",
        "theme6",
        "theme7",
        "theme8",
        "theme9"
    };

    private static string[] _promotionProducts =
    {
        "full_pack",
        "jr_pack1",
        "jr_pack2",
        "jr_pack3",
        "jr_pack4",
        "full_themes_pack",
        "themes_pack1",
        "themes_pack2",
        "themes_pack3",
        "themes_pack4",
        "themes_pack5",
        "themes_pack6"
    };

    private static string[] _extraProducts =
    {
        "play_now"
    };

    public static bool HasRestored { get; set; } = false;

    public static GameIAP Instance { get; private set; }
    private void Awake() => Instance = this;

    private void Start()
    {
        // If we haven't set up the Unity Purchasing reference
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing
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

        // Add a product to sell / restore by way of its identifier, associating the general identifier
        // with its store-specific identifiers.
        for (byte i = 0; i < _products.Length; i++)
        {
            builder.AddProduct(_products[i], ProductType.NonConsumable);
        }

        for (byte i = 0; i < _promotionProducts.Length; i++)
        {
            builder.AddProduct(_promotionProducts[i], ProductType.NonConsumable);
        }

        for (byte i = 0; i < _extraProducts.Length; i++)
        {
            builder.AddProduct(_extraProducts[i], ProductType.NonConsumable);
        }

        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);
    }

    // Only say we are initialized if both the Purchasing references are set.
    public bool IsInitialized() => m_StoreController != null && m_StoreExtensionProvider != null;

    // Buy the non-consumable product using its general identifier.
    // Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
    public void BuyProducts(byte n) => BuyProductID(_products[n]);
    public void BuyPromotionProducts(byte n) => BuyProductID(_promotionProducts[n]);
    public void BuyExtraProducts(byte n) => BuyProductID(_extraProducts[n]);

    public string GetPrice(byte n) => IsInitialized() ? m_StoreController.products.WithID(_products[n]).metadata.localizedPriceString : "";
    public string GetPromoPrice(byte n) => IsInitialized() ? m_StoreController.products.WithID(_promotionProducts[n]).metadata.localizedPriceString : "";
    public string GetExtraPrice(byte n) => IsInitialized() ? m_StoreController.products.WithID(_extraProducts[n]).metadata.localizedPriceString : "";

    private void BuyProductID(string productId)
    {
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            // ... look up the Product reference with the general product identifier and the Purchasing system's products collection.
            Product product = m_StoreController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
                ProcessingUI.Instance.OpenPanel("~Processing~");

                // ******Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase(product);
            }
            // Otherwise ...
            else
            {
                // ... report the product look-up failure situation  
                // ******Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                ProcessingUI.Instance.ClosePanel("~Failed to Buy~\nProduct not found");
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            // ******Debug.Log("BuyProductID FAIL. Not initialized.");
            ProcessingUI.Instance.ClosePanel("~Failed to Buy~\nNot initialized");
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
            // ******Debug.Log("RestorePurchases FAIL. Not initialized.");
            ProcessingUI.Instance.ClosePanel("~Restoration Failed~\nNot initialized");
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            // ******Debug.Log("RestorePurchases started ...");

            ProcessingUI.Instance.OpenPanel("~Restoring~");

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) => {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
                // ******Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");

                // This will take a little while for it to be true (Need to run through ProcessPurchase method), but false is quite quick
                if (result)
                    ProcessingUI.Instance.ClosePanel(HasRestored ? "Restored!" : "Nothing to restore!");
                else
                    ProcessingUI.Instance.ClosePanel("~Restoration Failed~");
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            // ******Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            ProcessingUI.Instance.ClosePanel("~Restoration Failed~\nPlatform not supported");
        }
    }


    //  
    // --- IStoreListener
    //

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        // ******Debug.Log("OnInitialized: PASS, refresh purchase text also");

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;

        // Refersh purchase button text
        GameTheme.Instance.RefreshCurSlot();
        FirstPurchaseButton.Instance.RefreshFirstShop();
        ExtraPurchaseButton.Instance.RefreshButton();
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        // ******Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        // Products has been purchased by the user...

        // **********For normal stuffs**********
        if (string.Equals(args.purchasedProduct.definition.id, _products[0], StringComparison.Ordinal))
        {
            FirstPurchaseButton.HaveJustRevive = true;
            // Save HaveJustRevive to save file
            GameData.SaveString(FirstPurchaseButton.HaveJustRevive.ToString(), "HaveJustRevive");

            // Update Game Stuff
            FirstPurchaseButton.Instance.RefreshReviveButton();
            FirstPurchaseButton.Instance.RefreshFirstShop();
            goto End;
        }

        for (int i = 1; i < _products.Length; i++)
        {
            if (string.Equals(args.purchasedProduct.definition.id, _products[i], StringComparison.Ordinal))
            {
                SecondPurchaseButton.HaveTheme[i - 1] = true;
                // Save HaveTheme to save file
                GameData.SaveBoolArray(SecondPurchaseButton.HaveTheme, "HaveTheme");

                // Update Game Stuff
                if (GamePromotion.CurShowing != -1 && GamePromotion.CheckTheme(GamePromotion.CurShowing, false) == false)
                {
                    GamePromotion.SetPromotion(-1);
                }
                else
                {
                    GameTheme.Instance.RefreshCurSlot();
                }
                goto End;
            }
        }

        // **********For promotion stuffs**********
        if (string.Equals(args.purchasedProduct.definition.id, _promotionProducts[0], StringComparison.Ordinal))
        {
            FirstPurchaseButton.HaveJustRevive = true;
            // Save HaveJustRevive to save file
            GameData.SaveString(FirstPurchaseButton.HaveJustRevive.ToString(), "HaveJustRevive");

            for (int i = 0; i < SecondPurchaseButton.HaveTheme.Length; i++)
                SecondPurchaseButton.HaveTheme[i] = true;
            // Save HaveTheme to save file
            GameData.SaveBoolArray(SecondPurchaseButton.HaveTheme, "HaveTheme");

            // Update Game Stuff
            FirstPurchaseButton.Instance.RefreshReviveButton();
            goto EndPromotion;
        }

        int[,] num1 = { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };
        for (int i = 1; i < 5; i++)
        {
            if (string.Equals(args.purchasedProduct.definition.id, _promotionProducts[i], StringComparison.Ordinal))
            {
                FirstPurchaseButton.HaveJustRevive = true;
                // Save HaveJustRevive to save file
                GameData.SaveString(FirstPurchaseButton.HaveJustRevive.ToString(), "HaveJustRevive");

                SecondPurchaseButton.HaveTheme[num1[i - 1, 0]] = true;
                SecondPurchaseButton.HaveTheme[num1[i - 1, 1]] = true;
                // Save HaveTheme to save file
                GameData.SaveBoolArray(SecondPurchaseButton.HaveTheme, "HaveTheme");

                // Update Game Stuff
                FirstPurchaseButton.Instance.RefreshReviveButton();
                goto EndPromotion;
            }
        }

        if (string.Equals(args.purchasedProduct.definition.id, _promotionProducts[5], StringComparison.Ordinal))
        {
            for (int i = 0; i < SecondPurchaseButton.HaveTheme.Length; i++)
                SecondPurchaseButton.HaveTheme[i] = true;
            // Save HaveTheme to save file
            GameData.SaveBoolArray(SecondPurchaseButton.HaveTheme, "HaveTheme");

            // Update Game Stuff
            goto EndPromotion;
        }

        int[,] num2 = { { 3, 4, 5 }, { 6, 7, 8 }, { 1, 2, 5 }, { 1, 2, 3 }, { 4, 7, 8 }, { 4, 5, 6 } };
        for (int i = 6; i < 12; i++)
        {
            if (string.Equals(args.purchasedProduct.definition.id, _promotionProducts[i], StringComparison.Ordinal))
            {
                SecondPurchaseButton.HaveTheme[num2[i - 6, 0]] = true;
                SecondPurchaseButton.HaveTheme[num2[i - 6, 1]] = true;
                SecondPurchaseButton.HaveTheme[num2[i - 6, 2]] = true;
                // Save HaveTheme to save file
                GameData.SaveBoolArray(SecondPurchaseButton.HaveTheme, "HaveTheme");

                // Update Game Stuff
                goto EndPromotion;
            }
        }

        // **********For Extra Products**********
        if (string.Equals(args.purchasedProduct.definition.id, _extraProducts[0], StringComparison.Ordinal))
        {
            ExtraPurchaseButton.HavePlayNow = true;
            // Save HavePlayNow to save file
            GameData.SaveString(ExtraPurchaseButton.HavePlayNow.ToString(), "HavePlayNow");

            GameSocial.UnlockAchievement(5);

            // ClosePanel with setting stuff back & Refresh Buttons
            ExtraPurchaseButton.Instance.CloseAndSetBack();
            ModeUI.Instance.RefreshButtons();
            // This will give sound & update mode info text
            ModeUI.Instance.Fetch(-1, "", "");

            goto Skip;
        }

    EndPromotion:
        GamePromotion.SetPromotion(-1); // remove promotion & update AllUI, CurSlot, FirstShop
    End:
        GamePromotion.UpdateHavePromotion(); // check if there is a promotion to show or not
    Skip:

        // If player hadn't bought anything, code wouldn't run inside this method
        HasRestored = true;

        // Only run in this method when they had purchased something, so no need to display this text
        if (ProcessingUI.Instance.InfoText.text == "~Processing~")
            ProcessingUI.Instance.ClosePanel("Successfully purchased!");

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        return PurchaseProcessingResult.Complete;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        // ******Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));

        // These will get called when it is failed to purchase from BuyProductsID
        string output = "~Unknown error has occurred~";

        switch (failureReason)
        {
            case PurchaseFailureReason.UserCancelled:
                output = "Cancelled!";
                break;
            case PurchaseFailureReason.ExistingPurchasePending:
                output = "~Purchase Failed~\nExisting Purchase Pending";
                break;
            case PurchaseFailureReason.PaymentDeclined:
                output = "~Purchase Failed~\nPayment Declined";
                break;
            case PurchaseFailureReason.SignatureInvalid:
                output = "~Purchase Failed~\nSignature Invalid";
                break;
            case PurchaseFailureReason.PurchasingUnavailable:
                output = "~Purchase Failed~\nPurchasing Unavailable";
                break;
            case PurchaseFailureReason.ProductUnavailable:
                output = "~Purchase Failed~\nProduct Unavailable";
                break;
            case PurchaseFailureReason.DuplicateTransaction:
                output = "~Purchase Failed~\nDuplicate Transaction";
                break;
            case PurchaseFailureReason.Unknown:
                output = "~Purchase Failed~\nNo connection";
                break;
        }

        ProcessingUI.Instance.ClosePanel(output);
    }
}
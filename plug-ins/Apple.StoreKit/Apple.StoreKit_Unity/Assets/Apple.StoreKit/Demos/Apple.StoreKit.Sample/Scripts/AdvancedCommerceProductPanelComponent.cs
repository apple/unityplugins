using System;
using Apple.StoreKit;
using UnityEngine;
using UnityEngine.UI;

public class AdvancedCommerceProductPanelComponent : MonoBehaviour
{
    [Header("UI References")]
    public Text statusText;
    public InputField productIdInput;
    public Button loadButton;
    public Text loadedProductIdText;
    public InputField compactJWSInput;
    public Button purchaseButton;

#if !UNITY_EDITOR && (UNITY_IOS || UNITY_TVOS || UNITY_VISIONOS)
    private AdvancedCommerceProduct _loadedProduct;
#endif

    void Start()
    {
        loadButton?.onClick.AddListener(OnLoadClick);
        purchaseButton?.onClick.AddListener(OnPurchaseClick);

        if (purchaseButton != null) purchaseButton.interactable = false;

#if UNITY_EDITOR || UNITY_STANDALONE_OSX
        if (loadButton != null) loadButton.interactable = false;
        SetStatus("AdvancedCommerceProduct: iOS 18.4+ / tvOS 18.4+ / visionOS 2.4+ only");
#else
        SetStatus("Enter a product ID and tap Load.");
#endif
    }

#if !UNITY_EDITOR && (UNITY_IOS || UNITY_TVOS || UNITY_VISIONOS)
    private async void OnLoadClick()
    {
        string productId = productIdInput != null ? productIdInput.text.Trim() : string.Empty;
        if (string.IsNullOrEmpty(productId))
        {
            SetStatus("Enter a product ID first.");
            return;
        }

        SetStatus($"Loading '{productId}'...");
        if (purchaseButton != null) purchaseButton.interactable = false;
        if (loadedProductIdText != null) loadedProductIdText.text = string.Empty;

        try
        {
            _loadedProduct?.Dispose();
            _loadedProduct = await AdvancedCommerceProduct.Load(productId);

            if (_loadedProduct == null)
            {
                SetStatus("Product not found.");
            }
            else
            {
                if (loadedProductIdText != null) loadedProductIdText.text = _loadedProduct.Id;
                if (purchaseButton != null) purchaseButton.interactable = true;
                SetStatus($"Loaded: {_loadedProduct.Id}");
            }
        }
        catch (Exception ex)
        {
            SetStatus($"Load error: {ex.Message}");
            Debug.LogError($"AdvancedCommerceProductPanelComponent: Load failed: {ex}");
        }
    }

    private async void OnPurchaseClick()
    {
        if (_loadedProduct == null)
        {
            SetStatus("Load a product first.");
            return;
        }

        string jws = compactJWSInput != null ? compactJWSInput.text.Trim() : string.Empty;
        if (string.IsNullOrEmpty(jws))
        {
            SetStatus("Enter a compact JWS token first.");
            return;
        }

        SetStatus($"Purchasing {_loadedProduct.Id}...");
        if (purchaseButton != null) purchaseButton.interactable = false;

        try
        {
            var result = await _loadedProduct.Purchase(jws);
            bool verified = result.Result == PurchaseResult.ResultEnum.Success && result.TransactionVerification.IsVerified;
            Debug.Log($"AdvancedCommerceProductPanelComponent: Purchase result={result.Result} verified={verified}");

            if (result.Result == PurchaseResult.ResultEnum.Success && verified)
            {
                using var transaction = result.TransactionVerification.SafePayload;
                transaction.Finish();
                SetStatus($"Purchased. Transaction {transaction.Id} finished.");
            }
            else
            {
                SetStatus($"Result: {result.Result}, Verified: {result.TransactionVerification.IsVerified}");
            }
        }
        catch (Exception ex)
        {
            SetStatus($"Purchase error: {ex.Message}");
            Debug.LogError($"AdvancedCommerceProductPanelComponent: Purchase failed: {ex}");
        }
        finally
        {
            if (purchaseButton != null) purchaseButton.interactable = true;
        }
    }

    void OnDisable()
    {
        _loadedProduct?.Dispose();
        _loadedProduct = null;
    }
#else
    private void OnLoadClick() { }
    private void OnPurchaseClick() { }
#endif

    private void SetStatus(string msg)
    {
        Debug.Log($"AdvancedCommerceProductPanelComponent: {msg}");
        if (statusText != null)
            statusText.text = msg;
    }
}

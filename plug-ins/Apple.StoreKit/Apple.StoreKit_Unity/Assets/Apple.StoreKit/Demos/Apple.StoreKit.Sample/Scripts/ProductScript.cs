#pragma warning disable CS1998
using System;
using System.Collections.Generic;
using Apple.StoreKit;
using UnityEngine;
using UnityEngine.UI;

public class ProductScript : MonoBehaviour
{
    public Text text;
    public Text status;
    public Button purchaseButton;
    public Button getLatestButton;
    public InputField quantityInput;
    public Toggle simulateAskToBuyToggle;

    private Product _product;

    void Start()
    {
        getLatestButton?.onClick.AddListener(OnGetLatestClick);
    }

    void Update()
    {
    }

    public void SetProductInfo(string name, string desc, string prc)
    {
        if (text != null)
            text.text = $"{name}:{desc}, {prc}";

        if (purchaseButton != null)
        {
            purchaseButton.interactable = false;
            purchaseButton.onClick.RemoveAllListeners();
        }

        if (getLatestButton != null) getLatestButton.interactable = false;
    }

    public void SetProductInfo(Product product)
    {
        _product = product;
        SetProductInfo(product.DisplayName, product.Type.LocalizedDescription, product.DisplayPrice);
        if (purchaseButton != null)
        {
            purchaseButton.interactable = true;
            purchaseButton.onClick.RemoveAllListeners();
            purchaseButton.onClick.AddListener(async () =>
            {
                try
                {
                    if (status != null)
                        status.text = $"Purchasing {product.Id}...";

                    Debug.Log($"ProductScript: Purchase {product.Id}");

                    var options = BuildPurchaseOptions();
                    var result = await product.Purchase(options);
                    var isVerified = result.Result == PurchaseResult.ResultEnum.Success && result.TransactionVerification.IsVerified;
                    Debug.Log($"ProductScript: Purchase completed for {product.Id}. Result: {Enum.GetName(typeof(PurchaseResult.ResultEnum), result.Result)}. Verified: {isVerified}");

                    if (result.Result == PurchaseResult.ResultEnum.Success && isVerified)
                    {
                        using var transaction = result.TransactionVerification.SafePayload;
                        transaction.Finish();
                        Debug.Log($"ProductScript: Transaction {transaction.Id} finished.");
                    }

                    if (status != null)
                        status.text = $"{product.Id} purchased. Result: {Enum.GetName(typeof(PurchaseResult.ResultEnum), result.Result)}. Verified: {result.TransactionVerification.IsVerified}";
                }
                catch (Exception ex)
                {
                    Debug.LogError($"ProductScript: Purchase failed for {product.Id} with error: {ex.Message}");
                    if (status != null) status.text = $"Error: {ex.Message}";
                }
            });
        }

        if (getLatestButton != null) getLatestButton.interactable = true;
    }

    public void SetTestProduct(string id, string displayName, string description, decimal price)
    {
        Debug.Log($"ProductScript: SetTestProduct {id}, {displayName}, ${price}");
        SetProductInfo(displayName, description, $"${price:F2}");
    }

    private PurchaseOption[] BuildPurchaseOptions()
    {
        var opts = new List<PurchaseOption>();

        if (quantityInput != null && int.TryParse(quantityInput.text, out int qty) && qty > 1)
            opts.Add(PurchaseOption.Quantity(qty));

        if (simulateAskToBuyToggle != null && simulateAskToBuyToggle.isOn)
            opts.Add(PurchaseOption.SimulatesAskToBuyInSandbox(true));

        return opts.ToArray();
    }

    private async void OnGetLatestClick()
    {
        if (_product == null) return;
        try
        {
            if (status != null) status.text = "Fetching latest transaction...";
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_TVOS || UNITY_VISIONOS || UNITY_STANDALONE_OSX)
            var vr = await _product.GetLatestTransaction();
            if (vr == null)
            {
                if (status != null) status.text = "No transaction found";
            }
            else if (!vr.IsVerified)
            {
                if (status != null) status.text = "Latest transaction: unverified";
            }
            else
            {
                using var transaction = vr.SafePayload;
                if (status != null) status.text = $"Latest: ID={transaction.Id} {transaction.ProductId}";
                Debug.Log($"ProductScript: Latest transaction {transaction.Id}");
            }
#else
            if (status != null) status.text = "GetLatestTransaction not available in editor";
#endif
        }
        catch (Exception ex)
        {
            Debug.LogError($"ProductScript: GetLatestTransaction error: {ex.Message}");
            if (status != null) status.text = $"Error: {ex.Message}";
        }
    }
}

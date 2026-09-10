using System;
using System.Linq;
using Apple.StoreKit;
using UnityEngine;
using UnityEngine.UI;

public class ProductPromotionInfoPanelComponent : MonoBehaviour
{
    [Header("UI References")]
    public Text statusText;
    public Text orderText;
    public Button getOrderButton;
    public Button resetOrderButton;
    public Button hideFirstButton;
    public Button showFirstButton;
    public Button updateAllButton;

    // Comma-separated product IDs to use for ordering/updating
    public InputField productIdsInput;

    void Start()
    {
        getOrderButton?.onClick.AddListener(OnGetOrderClick);
        resetOrderButton?.onClick.AddListener(OnResetOrderClick);
        hideFirstButton?.onClick.AddListener(OnHideFirstClick);
        showFirstButton?.onClick.AddListener(OnShowFirstClick);
        updateAllButton?.onClick.AddListener(OnUpdateAllClick);

#if UNITY_EDITOR || !UNITY_IOS
        if (getOrderButton != null) getOrderButton.interactable = false;
        if (resetOrderButton != null) resetOrderButton.interactable = false;
        if (hideFirstButton != null) hideFirstButton.interactable = false;
        if (showFirstButton != null) showFirstButton.interactable = false;
        if (updateAllButton != null) updateAllButton.interactable = false;
        SetStatus("ProductPromotionInfo: iOS 16.4+ only");
#endif
    }

    private string[] GetProductIds()
    {
        if (productIdsInput == null || string.IsNullOrWhiteSpace(productIdsInput.text))
            return new[] { "Unity.StoreKit.Consumable", "Unity.StoreKit.NonConsumable", "Unity.StoreKit.Subscription" };
        return productIdsInput.text.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToArray();
    }

    private async void OnGetOrderClick()
    {
        SetStatus("Getting current order...");
        try
        {
            var order = await ProductPromotionInfo.GetCurrentOrder();
            if (order.Length == 0)
            {
                SetStatus("No custom order set (using App Store Connect default)");
                if (orderText != null) orderText.text = "(default)";
            }
            else
            {
                string orderStr = string.Join(", ", order);
                SetStatus($"Current order ({order.Length} items)");
                if (orderText != null) orderText.text = orderStr;
            }
        }
        catch (Exception ex)
        {
            SetStatus($"Error: {ex.Message}");
        }
    }

    private async void OnResetOrderClick()
    {
        SetStatus("Resetting to App Store Connect order...");
        try
        {
            await ProductPromotionInfo.UpdateProductOrder(Array.Empty<string>());
            SetStatus("Order reset to App Store Connect default");
            if (orderText != null) orderText.text = "(default)";
        }
        catch (Exception ex)
        {
            SetStatus($"Error: {ex.Message}");
        }
    }

    private async void OnHideFirstClick()
    {
        var ids = GetProductIds();
        if (ids.Length == 0) { SetStatus("Enter product IDs first"); return; }
        SetStatus($"Hiding {ids[0]}...");
        try
        {
            await ProductPromotionInfo.UpdateProductVisibility(ids[0], ProductPromotionInfo.Visibility.Hidden);
            SetStatus($"Hidden: {ids[0]}");
        }
        catch (Exception ex)
        {
            SetStatus($"Error: {ex.Message}");
        }
    }

    private async void OnShowFirstClick()
    {
        var ids = GetProductIds();
        if (ids.Length == 0) { SetStatus("Enter product IDs first"); return; }
        SetStatus($"Showing {ids[0]}...");
        try
        {
            await ProductPromotionInfo.UpdateProductVisibility(ids[0], ProductPromotionInfo.Visibility.Visible);
            SetStatus($"Visible: {ids[0]}");
        }
        catch (Exception ex)
        {
            SetStatus($"Error: {ex.Message}");
        }
    }

    private async void OnUpdateAllClick()
    {
        var ids = GetProductIds();
        if (ids.Length == 0) { SetStatus("Enter product IDs first"); return; }
        var visibilities = new ProductPromotionInfo.Visibility[ids.Length];
        visibilities[0] = ProductPromotionInfo.Visibility.Visible;
        for (int i = 1; i < visibilities.Length; i++)
            visibilities[i] = ProductPromotionInfo.Visibility.AppStoreConnectDefault;
        SetStatus("Calling UpdateAll...");
        try
        {
            await ProductPromotionInfo.UpdateAll(ids, visibilities);
            SetStatus($"UpdateAll complete: {ids[0]}=Visible, others=Default");
        }
        catch (Exception ex)
        {
            SetStatus($"Error: {ex.Message}");
        }
    }

    private void SetStatus(string msg)
    {
        Debug.Log($"ProductPromotionInfoPanelComponent: {msg}");
        if (statusText != null)
            statusText.text = msg;
    }
}

using System;
using Apple.StoreKit;
using UnityEngine;
using UnityEngine.UI;

public class AppStorePanelComponent : MonoBehaviour
{
    [Header("UI References")]
    public Text statusText;
    public Button syncButton;
    public Button requestReviewButton;
    public Button canMakePaymentsButton;
    public Button presentOfferCodeButton;
    public Button showManageSubsButton;
    public Button showManageSubsGroupButton;
    public InputField subscriptionGroupIdInput;
    public Button storefrontButton;
    public Text storefrontText;

    void Start()
    {
        syncButton?.onClick.AddListener(OnSyncClick);
        requestReviewButton?.onClick.AddListener(OnRequestReviewClick);
        canMakePaymentsButton?.onClick.AddListener(OnCanMakePaymentsClick);
        presentOfferCodeButton?.onClick.AddListener(OnPresentOfferCodeClick);
        showManageSubsButton?.onClick.AddListener(OnShowManageSubsClick);
        showManageSubsGroupButton?.onClick.AddListener(OnShowManageSubsGroupClick);
        storefrontButton?.onClick.AddListener(OnStorefrontClick);

#if UNITY_EDITOR
        if (presentOfferCodeButton != null) presentOfferCodeButton.interactable = false;
        if (showManageSubsButton != null) showManageSubsButton.interactable = false;
        if (showManageSubsGroupButton != null) showManageSubsGroupButton.interactable = false;
#endif
    }

    private async void OnSyncClick()
    {
        SetStatus("Syncing...");
        try
        {
            await AppStore.Sync();
            SetStatus("Sync complete");
        }
        catch (Exception ex)
        {
            SetStatus($"Sync error: {ex.Message}");
        }
    }

    private void OnRequestReviewClick()
    {
#if !UNITY_EDITOR
        AppStore.RequestReview();
        SetStatus("Review requested");
#else
        SetStatus("RequestReview not available in editor");
#endif
    }

    private void OnCanMakePaymentsClick()
    {
        bool can = AppStore.CanMakePayments;
        SetStatus($"CanMakePayments: {can}");
    }

    private async void OnPresentOfferCodeClick()
    {
        SetStatus("Presenting offer code sheet...");
        try
        {
            await AppStore.PresentOfferCodeRedeemSheet();
            SetStatus("Offer code sheet dismissed");
        }
        catch (Exception ex)
        {
            SetStatus($"Error: {ex.Message}");
        }
    }

    private async void OnShowManageSubsClick()
    {
        SetStatus("Opening manage subscriptions...");
        try
        {
            await AppStore.ShowManageSubscriptions();
            SetStatus("Manage subscriptions dismissed");
        }
        catch (Exception ex)
        {
            SetStatus($"Error: {ex.Message}");
        }
    }

    private async void OnShowManageSubsGroupClick()
    {
        string groupId = subscriptionGroupIdInput != null ? subscriptionGroupIdInput.text : string.Empty;
        if (string.IsNullOrEmpty(groupId))
        {
            SetStatus("Enter a subscription group ID first");
            return;
        }
        SetStatus($"Opening manage subscriptions for group {groupId}...");
        try
        {
            await AppStore.ShowManageSubscriptions(groupId);
            SetStatus("Manage subscriptions dismissed");
        }
        catch (Exception ex)
        {
            SetStatus($"Error: {ex.Message}");
        }
    }

#if UNITY_EDITOR
    private void OnStorefrontClick()
    {
        SetStatus("Storefront not available in editor");
    }
#else
    private async void OnStorefrontClick()
    {
        SetStatus("Fetching storefront...");
        try
        {
            using var storefront = await Storefront.GetCurrent();
            if (storefront == null)
            {
                SetStatus("Storefront: unavailable");
                if (storefrontText != null) storefrontText.text = string.Empty;
            }
            else
            {
                var text = $"ID: {storefront.Id}  Country: {storefront.CountryCode}";
                SetStatus("Storefront fetched");
                if (storefrontText != null) storefrontText.text = text;
                else SetStatus(text);
            }
        }
        catch (Exception ex)
        {
            SetStatus($"Storefront error: {ex.Message}");
        }
    }
    #endif

    private void SetStatus(string msg)
    {
        Debug.Log($"AppStorePanelComponent: {msg}");
        if (statusText != null)
            statusText.text = msg;
    }
}

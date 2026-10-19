using System;
using System.Text;
using Apple.StoreKit;
using UnityEngine;
using UnityEngine.UI;

public class SubscriptionStatusPanelComponent : MonoBehaviour
{
    [Header("UI References")]
    public Text statusText;
    public Text renewalInfoText;
    public Button fetchButton;

    // Comma-separated product IDs for subscriptions to check
    public InputField productIdsInput;

    void Start()
    {
        fetchButton?.onClick.AddListener(OnFetchClick);
        SetStatus("Enter subscription product IDs and tap Fetch");
    }

#if UNITY_EDITOR
    private void OnFetchClick()
    {
        SetStatus("Subscription status not available in editor");
    }
#else
    private async void OnFetchClick()
    {
        string input = productIdsInput != null ? productIdsInput.text : string.Empty;
        string[] ids = string.IsNullOrWhiteSpace(input)
            ? new[] { "Unity.StoreKit.Subscription" }
            : input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        SetStatus("Fetching subscription status...");
        if (renewalInfoText != null) renewalInfoText.text = string.Empty;

        try
        {
            var products = await Product.FetchProducts(ids);
            if (products.Count == 0)
            {
                SetStatus("No products found for given IDs");
                return;
            }

            var sb = new StringBuilder();
            foreach (var product in products)
            {
                var subInfo = product.Subscription;
                if (subInfo == null)
                {
                    sb.AppendLine($"{product.Id}: not a subscription");
                    continue;
                }

                bool eligible = await subInfo.IsEligibleForIntroOffer();
                sb.AppendLine($"[{product.Id}] EligibleForIntroOffer: {eligible}");

                var winBackOffers = subInfo.WinBackOffers;
                if (winBackOffers.Length > 0)
                {
                    sb.AppendLine($"  WinBackOffers ({winBackOffers.Length}):");
                    foreach (var wo in winBackOffers)
                        sb.AppendLine($"    Id={wo.Id ?? "null"} {wo.PaymentMode} {wo.PeriodValue}{wo.PeriodUnit} x{wo.PeriodCount} {wo.DisplayPrice}");
                }

                SubscriptionStatus[] statuses;
                try { statuses = await subInfo.GetStatus(); }
                catch (Exception ex)
                {
                    sb.AppendLine($"{product.Id}: error - {ex.Message}");
                    continue;
                }

                if (statuses.Length == 0)
                {
                    sb.AppendLine($"{product.Id}: no status entries");
                    continue;
                }

                foreach (var s in statuses)
                {
                    sb.AppendLine($"  State: {s.State}");

                    var vrRenewal = s.RenewalInfo;
                    if (!vrRenewal.IsVerified)
                    {
                        sb.AppendLine("  RenewalInfo: unverified");
                        continue;
                    }

                    var ri = vrRenewal.SafePayload;
                    sb.AppendLine($"    CurrentProductId: {ri.CurrentProductId}");
                    sb.AppendLine($"    WillAutoRenew: {ri.WillAutoRenew}");
                    sb.AppendLine($"    IsInBillingRetry: {ri.IsInBillingRetry}");

                    if (ri.ExpirationReason.HasValue)
                        sb.AppendLine($"    ExpirationReason: {ri.ExpirationReason}");
                    if (!string.IsNullOrEmpty(ri.OfferId))
                        sb.AppendLine($"    OfferId: {ri.OfferId}");
                    if (ri.OfferType.HasValue)
                        sb.AppendLine($"    OfferType: {ri.OfferType}");
                    if (ri.GracePeriodExpirationDate.HasValue)
                        sb.AppendLine($"    GracePeriodExpires: {ri.GracePeriodExpirationDate:yyyy-MM-dd HH:mm}");
                }
            }

            if (renewalInfoText != null) renewalInfoText.text = sb.ToString();
            SetStatus("Done");
        }
        catch (Exception ex)
        {
            SetStatus($"Error: {ex.Message}");
            Debug.LogError($"SubscriptionStatusPanelComponent: {ex}");
        }
    }
    #endif

    private void SetStatus(string msg)
    {
        Debug.Log($"SubscriptionStatusPanelComponent: {msg}");
        if (statusText != null)
            statusText.text = msg;
    }
}

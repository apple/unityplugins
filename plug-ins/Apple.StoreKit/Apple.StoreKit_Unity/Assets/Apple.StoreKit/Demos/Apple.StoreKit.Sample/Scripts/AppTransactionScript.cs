using System.Threading.Tasks;
using Apple.StoreKit;
using UnityEngine;
using UnityEngine.UI;

public class AppTransactionScript : MonoBehaviour
{
    public Text text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

#if !UNITY_EDITOR
    public async void OnEnable()
    {
        Debug.Log("AppTransaction OnEnable");
        try
        {
            var verificationResult = await AppTransaction.GetShared();

            if (verificationResult.IsVerified)
            {
                var appTransaction = verificationResult.SafePayload;
                Debug.Log($"AppTransaction BundleId: {appTransaction.BundleId}");
                text.text = $@"
Verification Status: Verified ✓
App Transaction ID: {appTransaction.AppTransactionID}
Bundle ID: {appTransaction.BundleId}
Environment: {appTransaction.Environment}
Original Platform: {appTransaction.OriginalPlatform}
Original App Version: {appTransaction.OriginalAppVersion}
Purchase Date: {appTransaction.OriginalPurchaseDate}
Device Verification Nonce: {appTransaction.DeviceVerificationNonce}
JWS Available: Yes
            ";
            }
            else
            {
                var appTransaction = verificationResult.UnsafePayload;
                var error = verificationResult.VerificationError;
                Debug.LogWarning($"AppTransaction verification failed: {error?.LocalizedDescription}");
                text.text = $@"
Verification Status: Unverified ⚠️
Error: {error?.LocalizedDescription ?? "Unknown"}
Bundle ID: {appTransaction.BundleId}
Environment: {appTransaction.Environment}
Original Platform: {appTransaction.OriginalPlatform}
            ";
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"AppTransaction Error: {ex}");
            text.text = $"Error: {ex.Message}";
        }
    }
#else 
    public void OnEnable()
    {
        Debug.Log("AppTransaction OnEnable - Editor - skipping");
        text.text = "AppTransaction not available in Editor";
    }
#endif
}

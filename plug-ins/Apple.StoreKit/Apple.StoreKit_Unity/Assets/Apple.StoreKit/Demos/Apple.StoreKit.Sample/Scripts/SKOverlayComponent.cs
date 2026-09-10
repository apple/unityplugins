using Apple.StoreKit;
using UnityEngine;
using UnityEngine.UI;

public class SKOverlayComponent : MonoBehaviour
{
    public Text statusText;
    public Button presentButton;
    public Button dismissButton;

    private const string ShazamAppId = "897118787";

#if UNITY_IOS || UNITY_VISIONOS
    void Start()
    {
        presentButton?.onClick.AddListener(OnPresentClick);
        dismissButton?.onClick.AddListener(OnDismissClick);

        SKOverlay.WillStartPresentation += () => UpdateStatus("Overlay will start presenting...");
        SKOverlay.DidFinishPresentation += () => UpdateStatus("Overlay presented.");
        SKOverlay.WillStartDismissal += () => UpdateStatus("Overlay will start dismissing...");
        SKOverlay.DidFinishDismissal += () => UpdateStatus("Overlay dismissed.");
        SKOverlay.DidFailToLoad += (error) => UpdateStatus($"Overlay failed to load: {error.Message}");
    }

    private void OnPresentClick()
    {
        UpdateStatus("Presenting SKOverlay for Shazam...");
        SKOverlay.Present(ShazamAppId, SKOverlayPosition.Bottom);
    }

    private void OnDismissClick()
    {
        UpdateStatus("Dismissing SKOverlay...");
        SKOverlay.Dismiss();
    }

    private void UpdateStatus(string message)
    {
        Debug.Log($"SKOverlay: {message}");
        if (statusText != null)
            statusText.text = message;
    }
#endif
}

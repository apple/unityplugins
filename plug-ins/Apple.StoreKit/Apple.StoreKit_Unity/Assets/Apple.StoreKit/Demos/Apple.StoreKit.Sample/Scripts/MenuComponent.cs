using Apple.StoreKit;
using UnityEngine;
using UnityEngine.UI;

public class MenuComponent : MonoBehaviour
{
    public Button transactionsButton;
    public GameObject transactionsPanel;
    public Button productsButton;
    public GameObject productsPanel;
    public Button overlayButton;
    public GameObject menuPanel;

    public Button appStoreButton;
    public GameObject appStorePanel;
    public Button promotionInfoButton;
    public GameObject promotionInfoPanel;
    public Button subscriptionStatusButton;
    public GameObject subscriptionStatusPanel;
    public Button eventsButton;
    public GameObject eventsPanel;
    public Button advancedCommerceButton;
    public GameObject advancedCommercePanel;

    public NavigationComponent navigationComponent;

    void Start()
    {
        transactionsButton?.onClick.AddListener(OnTransactionsClick);
        productsButton?.onClick.AddListener(OnProductsClick);
        appStoreButton?.onClick.AddListener(() => navigationComponent.NavigateTo(appStorePanel));
        promotionInfoButton?.onClick.AddListener(() => navigationComponent.NavigateTo(promotionInfoPanel));
        subscriptionStatusButton?.onClick.AddListener(() => navigationComponent.NavigateTo(subscriptionStatusPanel));
        eventsButton?.onClick.AddListener(() => navigationComponent.NavigateTo(eventsPanel));
        advancedCommerceButton?.onClick.AddListener(() => navigationComponent.NavigateTo(advancedCommercePanel));

        if (overlayButton != null)
        {
#if UNITY_IOS || UNITY_VISIONOS
            overlayButton.onClick.AddListener(OnOverlayClick);
#else
            overlayButton.interactable = false;
#endif
        }
    }

    private void OnProductsClick() => navigationComponent.NavigateTo(productsPanel);
    private void OnTransactionsClick() => navigationComponent.NavigateTo(transactionsPanel);

#if UNITY_IOS || UNITY_VISIONOS
    private void OnOverlayClick()
    {
        SKOverlay.Present("897118787");
    }
#endif
}

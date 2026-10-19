using Apple.StoreKit;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EventsPanelComponent : MonoBehaviour
{
    [Header("Messages (iOS 16+ / visionOS 2.2+)")]
    public Text messagesStatusText;
    public Button startMessagesButton;
    public Button stopMessagesButton;
    public GameObject messageListParent;
    public GameObject messageItemPrefab;

    [Header("Purchase Intents (iOS 16.4+)")]
    public Text intentsStatusText;
    public Button startIntentsButton;
    public Button stopIntentsButton;
    public GameObject intentListParent;
    public GameObject intentItemPrefab;

    private bool _listeningMessages = false;
    private bool _listeningIntents = false;
    private readonly List<GameObject> _messageItems = new List<GameObject>();
    private readonly List<GameObject> _intentItems = new List<GameObject>();

    void Start()
    {
        startMessagesButton?.onClick.AddListener(OnStartMessagesClick);
        stopMessagesButton?.onClick.AddListener(OnStopMessagesClick);
        if (stopMessagesButton != null) stopMessagesButton.interactable = false;

        startIntentsButton?.onClick.AddListener(OnStartIntentsClick);
        stopIntentsButton?.onClick.AddListener(OnStopIntentsClick);
        if (stopIntentsButton != null) stopIntentsButton.interactable = false;

#if UNITY_EDITOR || (!UNITY_IOS && !UNITY_VISIONOS)
        if (startMessagesButton != null) startMessagesButton.interactable = false;
        SetMessagesStatus("Messages: iOS 16.0+ / visionOS 2.2+ only");
#else
        SetMessagesStatus("Not listening. Tap Start to receive App Store messages.");
#endif

#if UNITY_EDITOR || !UNITY_IOS
        if (startIntentsButton != null) startIntentsButton.interactable = false;
        SetIntentsStatus("Purchase Intents: iOS 16.4+ only");
#else
        SetIntentsStatus("Not listening. Tap Start to receive promoted IAP intents.");
#endif
    }

    void OnDisable()
    {
        if (_listeningMessages) StopMessages();
        if (_listeningIntents) StopIntents();
    }

    // --- Messages ---

    private void OnStartMessagesClick()
    {
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_VISIONOS)
        Message.MessageReceived += OnMessageReceived;
        _listeningMessages = true;
        if (startMessagesButton != null) startMessagesButton.interactable = false;
        if (stopMessagesButton != null) stopMessagesButton.interactable = true;
        SetMessagesStatus("Listening for App Store messages...");
#endif
    }

    private void OnStopMessagesClick() => StopMessages();

    private void StopMessages()
    {
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_VISIONOS)
        Message.MessageReceived -= OnMessageReceived;
#endif
        _listeningMessages = false;
        if (startMessagesButton != null) startMessagesButton.interactable = true;
        if (stopMessagesButton != null) stopMessagesButton.interactable = false;
        SetMessagesStatus("Stopped listening.");
    }

#if !UNITY_EDITOR && (UNITY_IOS || UNITY_VISIONOS)
    private async void OnMessageReceived(object sender, Message message)
    {
        Debug.Log($"EventsPanelComponent: Message reason={message.Reason}");
        SetMessagesStatus($"Message received: {message.Reason}");
        AddItemToList(messageListParent, messageItemPrefab, _messageItems, $"Reason: {message.Reason}");
        try
        {
            await message.Display();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"EventsPanelComponent: Message.Display error: {ex.Message}");
        }
    }
#endif

    // --- Purchase Intents ---

    private void OnStartIntentsClick()
    {
#if !UNITY_EDITOR && UNITY_IOS
        PurchaseIntent.Intents += OnPurchaseIntent;
        _listeningIntents = true;
        if (startIntentsButton != null) startIntentsButton.interactable = false;
        if (stopIntentsButton != null) stopIntentsButton.interactable = true;
        SetIntentsStatus("Listening for purchase intents...");
#endif
    }

    private void OnStopIntentsClick() => StopIntents();

    private void StopIntents()
    {
#if !UNITY_EDITOR && UNITY_IOS
        PurchaseIntent.Intents -= OnPurchaseIntent;
#endif
        _listeningIntents = false;
        if (startIntentsButton != null) startIntentsButton.interactable = true;
        if (stopIntentsButton != null) stopIntentsButton.interactable = false;
        SetIntentsStatus("Stopped listening.");
    }

#if !UNITY_EDITOR && UNITY_IOS
    private void OnPurchaseIntent(object sender, PurchaseIntent intent)
    {
        Debug.Log($"EventsPanelComponent: Intent id={intent.Id}, product={intent.Product?.Id}");
        SetIntentsStatus($"Intent received: {intent.Product?.Id ?? intent.Id}");
        string label = $"Intent: {intent.Id}\nProduct: {intent.Product?.DisplayName ?? intent.Product?.Id ?? "unknown"}";
        AddItemToList(intentListParent, intentItemPrefab, _intentItems, label);
    }
#endif

    // --- Helpers ---

    private void AddItemToList(GameObject parent, GameObject prefab, List<GameObject> list, string label)
    {
        if (parent == null || prefab == null) return;
        var obj = Instantiate(prefab, parent.transform);
        list.Add(obj);
        var txt = obj.GetComponentInChildren<Text>();
        if (txt != null) txt.text = label;
    }

    private void SetMessagesStatus(string msg)
    {
        Debug.Log($"EventsPanelComponent (messages): {msg}");
        if (messagesStatusText != null) messagesStatusText.text = msg;
    }

    private void SetIntentsStatus(string msg)
    {
        Debug.Log($"EventsPanelComponent (intents): {msg}");
        if (intentsStatusText != null) intentsStatusText.text = msg;
    }
}

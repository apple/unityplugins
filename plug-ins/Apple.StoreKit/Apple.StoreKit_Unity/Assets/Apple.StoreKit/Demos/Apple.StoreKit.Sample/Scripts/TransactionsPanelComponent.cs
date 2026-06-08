using UnityEngine;
using UnityEngine.UI;
using Apple.StoreKit;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

public class TransactionsPanelComponent : MonoBehaviour
{
    [Header("UI References")]
    public GameObject transactionListParent;
    public GameObject transactionItemPrefab;
    public Text statusText;

    [Header("Editor Testing")]
    public Button addTestTransactionButton;

    private List<GameObject> transactionItems = new List<GameObject>();
    private int testTransactionCounter = 0;

    private Task RetrieveTransactionsTask = null;
    private CancellationTokenSource retrieveCancellationTokenSource = new CancellationTokenSource();
    private bool updateIsHooked = false;
    private TransactionSource currentSource = TransactionSource.All;

    public enum TransactionSource
    {
        All,
        Unfinished,
        CurrentEntitlements,
        Updates
    }


    private TaskScheduler unityScheduler;

    void Awake()
    {
        unityScheduler = TaskScheduler.FromCurrentSynchronizationContext();
    }

    void Start()
    {
        // Clear status on start
        if (statusText != null)
            statusText.text = "Ready to retrieve transactions";

        if (addTestTransactionButton != null)
            addTestTransactionButton.onClick.AddListener(OnAddTestTransactionClick);
    }

    void OnEnable()
    {
        OnSourceChange((int)currentSource);
    }

    public void OnSourceChange(int intSource)
    {
        Debug.Log($"RetrieveTransactionsComponent: OnSourceChange: {intSource}");
        var source = (TransactionSource)intSource;

        retrieveCancellationTokenSource.Cancel();
        retrieveCancellationTokenSource = new CancellationTokenSource();
        currentSource = source;

        switch (source)
        {
            case TransactionSource.All:
            case TransactionSource.Unfinished:
            case TransactionSource.CurrentEntitlements:
                if (updateIsHooked)
                {
                    Transaction.Updates -= OnUpdate;
                    updateIsHooked = false;
                }

                RetrieveTransactionsTask = LoadTransactionsAsync(source);
                break;
            case TransactionSource.Updates:
                if (!updateIsHooked)
                {
                    if (statusText != null)
                        statusText.text = "Waiting on updates...";
                        
                    ClearTransactionList();
                    Transaction.Updates += OnUpdate;
                    updateIsHooked = true;
                }
                break;
            default:
                Debug.LogWarning("RetrieveTransactionsComponent: Unknown transaction source selected");
                break;
        }
    }

    public void OnUpdate(object _, VerificationResult<Transaction> result)
    {
        Debug.Log($"RetrieveTransactionsComponent: OnUpdate: Received transaction update IsVerified: {result.IsVerified}");
        if (!result.IsVerified)
            return;

        Task.Factory.StartNew(
            () => AddTransactionToList(result.SafePayload),
            CancellationToken.None,
            TaskCreationOptions.None,
            unityScheduler);
    }


    private async Task LoadTransactionsAsync(TransactionSource source)
    {
        if (statusText != null)
            statusText.text = "Retrieving transactions...";

        // Clear previous results
        ClearTransactionList();

        var errored = false;

        try
        {
            int transactionCount = 0;
            var enumerable = source switch
            {
                TransactionSource.All => Transaction.GetAll(),
                TransactionSource.Unfinished => Transaction.GetUnfinished(),
                TransactionSource.CurrentEntitlements => Transaction.GetCurrentEntitlements(),
                _ => Transaction.GetAll(),
            };

            await foreach (var result in enumerable.WithCancellation(retrieveCancellationTokenSource.Token))
            {
                if (!result.IsVerified)
                {
                    Debug.LogWarning("RetrieveTransactionsComponent: Skipping unverified transaction");
                    continue;
                }

                var transaction = result.SafePayload;
                
                transactionCount++;
                Debug.Log(transaction.ToString());

                // Add transaction to UI list
                AddTransactionToList(transaction);
            }

            Debug.Log("RetrieveTransactionsComponent: All transactions retrieved successfully");

            if (statusText != null)
            {
                if (transactionCount == 0)
                    statusText.text = "No transactions found";
                else
                    statusText.text = $"Retrieved {transactionCount} transaction(s)";
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"RetrieveTransactionsComponent: Error retrieving transactions: {ex.Message}");

            if (statusText != null)
                statusText.text = $"Error: {ex.Message}";

            errored = true;
        }
        finally
        {
            if (!errored && statusText != null)
                statusText.text += " (Completed)";
        }
    }

    private void ClearTransactionList()
    {
        foreach (var item in transactionItems)
        {
            if (item != null)
                DestroyImmediate(item);
        }
        transactionItems.Clear();
    }

    private void AddTransactionToList(Transaction transaction)
    {
        if (transactionListParent == null || transactionItemPrefab == null)
        {
            Debug.LogWarning("RetrieveTransactionsComponent: Missing UI references. Cannot display transaction in list.");
            return;
        }

        // Instantiate transaction item
        GameObject itemObj = Instantiate(transactionItemPrefab, transactionListParent.transform, worldPositionStays: false);
        transactionItems.Add(itemObj);

        // Configure the transaction item
        TransactionItem itemComponent = itemObj.GetComponent<TransactionItem>();
        if (itemComponent != null)
        {
            itemComponent.SetTransaction(transaction);
        }
        else
        {
            Debug.LogWarning("RetrieveTransactionsComponent: TransactionItem component not found on prefab.");
        }
    }

    public void OnAddTestTransactionClick()
    {
        Debug.Log("Adding test transaction");
        AddTestTransaction();
    }

    private void AddTestTransaction()
    {
        if (transactionListParent == null || transactionItemPrefab == null)
        {
            Debug.LogWarning("RetrieveTransactionsComponent: Missing UI references. Cannot display transaction in list.");
            return;
        }

        // Instantiate transaction item
        GameObject itemObj = Instantiate(transactionItemPrefab, transactionListParent.transform, worldPositionStays: false);
        transactionItems.Add(itemObj);

        // Force the height to 100 pixels
        RectTransform rectTransform = itemObj.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = new UnityEngine.Vector2(rectTransform.sizeDelta.x, 100);
        }

        // Configure the transaction item with test data
        TransactionItem itemComponent = itemObj.GetComponent<TransactionItem>();
        if (itemComponent != null)
        {
            testTransactionCounter++;
            string[] testProducts = { "Unity.StoreKit.Consumable", "Unity.StoreKit.NonConsumable", "Unity.StoreKit.Subscription" };
            string[] testStatuses = { "Active", "Upgraded", "Revoked" };

            itemComponent.SetTestTransaction(
                id: (ulong)testTransactionCounter,
                productId: testProducts[testTransactionCounter % testProducts.Length],
                purchaseDate: System.DateTime.Now.AddDays(-testTransactionCounter),
                status: testStatuses[testTransactionCounter % testStatuses.Length]
            );

            if (statusText != null)
                statusText.text = $"Added test transaction #{testTransactionCounter}";
        }
        else
        {
            Debug.LogWarning("RetrieveTransactionsComponent: TransactionItem component not found on prefab.");
        }
    }
}

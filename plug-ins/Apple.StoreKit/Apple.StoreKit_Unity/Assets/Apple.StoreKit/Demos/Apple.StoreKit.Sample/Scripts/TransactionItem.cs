using System;
using UnityEngine;
using UnityEngine.UI;
using Apple.StoreKit;

public class TransactionItem : MonoBehaviour
{
    public Text transactionText;
    public Button refundButton;

    private ulong _transactionId;

    void Start()
    {
#if UNITY_TVOS || UNITY_EDITOR
        if (refundButton != null) refundButton.interactable = false;
#else
        refundButton?.onClick.AddListener(OnRefundClick);
#endif
    }

    public void SetTransaction(Transaction transaction)
    {
        Debug.Log($"TransactionItem: {transaction}");
        transactionText.text = transaction.ToString();
        _transactionId = transaction.Id;
    }

    public void SetTestTransaction(ulong id, string productId, System.DateTime purchaseDate, string status)
    {
        Debug.Log($"TransactionItem: SetTestTransaction {id}, {productId}, {purchaseDate}");
        transactionText.text = $"ID: {id} Product: {productId} Date: {purchaseDate:yyyy-MM-dd HH:mm} Status: {status}";
        _transactionId = id;
    }

#if !UNITY_TVOS && !UNITY_EDITOR
    private async void OnRefundClick()
    {
        try
        {
            Debug.Log($"TransactionItem: BeginRefundRequest for {_transactionId}");
            var status = await Transaction.BeginRefundRequest(_transactionId);
            Debug.Log($"TransactionItem: RefundRequest status={status}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"TransactionItem: RefundRequest error: {ex.Message}");
        }
    }
#endif
}

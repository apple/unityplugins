using System.Collections.Generic;
using System.Threading.Tasks;
using Apple.StoreKit;
using UnityEngine;
using UnityEngine.UI;

public class ProductsPanelComponent : MonoBehaviour
{
    public Text statusText;
    public Button retrieveButton;
    public GameObject productItemPrefab;
    public GameObject productListParent;

    [Header("Editor Testing")]
    public Button addTestProductButton;

    private List<GameObject> productItems = new List<GameObject>();
    private int testProductCounter = 0;

    private static readonly string[] productIds = new string[]
    {
        "Unity.StoreKit.Consumable",
        "Unity.StoreKit.NonConsumable",
        "Unity.StoreKit.Subscription"
    };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (addTestProductButton != null)
            addTestProductButton.onClick.AddListener(OnAddTestProductClick);

        if (retrieveButton != null)
            retrieveButton.onClick.AddListener(async () =>
            {
                Debug.Log("ProductsPanelComponent: OnRetrieveClick");
                await RefreshList();
            });
    }

    // Update is called once per frame
    void Update()
    {

    }

    async void OnEnable()
    {
        await RefreshList();
    }

    private async Task RefreshList()
    {
        ClearList();
        Debug.Log("ProductsPanelComponent: RefreshList");
        statusText.text = "Loading...";

        var products = await Product.FetchProducts(productIds);
        Debug.Log("Products loaded: " + products.Count);
        foreach (var product in products)
        {
            Debug.Log($"Product: {product.Id}, Description: {product.Description}, Price: {product.Price}");
            AddProductToList(product);
        }

        statusText.text = $"Loaded {products.Count} products";
    }

    private void ClearList()
    {
        foreach (var item in productItems)
        {
            Destroy(item);
        }
        productItems.Clear();
    }

    private void AddProductToList(Product product)
    {
        var itemObj = Instantiate(productItemPrefab, productListParent.transform);
        var productScript = itemObj.GetComponent<ProductScript>();
        if (productScript != null)
        {
            productScript.SetProductInfo(product);
            productScript.status = statusText;
        }
        productItems.Add(itemObj);
    }


    void OnDisable()
    {
        Debug.Log("ProductsPanelComponent: OnDisable");
    }

    public void OnAddTestProductClick()
    {
        Debug.Log("Adding test product");
        AddTestProduct();
    }

    private void AddTestProduct()
    {
        if (productListParent == null || productItemPrefab == null)
        {
            Debug.LogWarning("ProductsPanelComponent: Missing UI references. Cannot display product in list.");
            return;
        }

        // Instantiate product item
        var itemObj = Instantiate(productItemPrefab, productListParent.transform, worldPositionStays: false);
        productItems.Add(itemObj);

        // Force the height to 100 pixels
        RectTransform rectTransform = itemObj.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = new UnityEngine.Vector2(rectTransform.sizeDelta.x, 100);
        }

        // Configure the product item with test data
        var productScript = itemObj.GetComponent<ProductScript>();
        if (productScript != null)
        {
            testProductCounter++;
            string[] testProductIds = { "Unity.StoreKit.Consumable", "Unity.StoreKit.NonConsumable", "Unity.StoreKit.Subscription" };
            string[] testDescriptions = { "Test Consumable Product", "Test Non-Consumable Product", "Test Subscription Product" };
            decimal[] testPrices = { 0.99m, 4.99m, 9.99m };

            int index = testProductCounter % testProductIds.Length;

            productScript.SetTestProduct(
                id: testProductIds[index],
                displayName: $"Test Product {testProductCounter}",
                description: testDescriptions[index],
                price: testPrices[index]
            );

            if (statusText != null)
                statusText.text = $"Added test product #{testProductCounter}";
        }
        else
        {
            Debug.LogWarning("ProductsPanelComponent: ProductScript component not found on prefab.");
        }
    }
}

# Apple - StoreKit

## Overview
The Apple.StoreKit Unity Plug-In exposes Apple's StoreKit 2 framework to Unity developers, providing a modern C# API for implementing in-app purchases, subscriptions, and transaction management in your iOS, macOS, tvOS, and visionOS apps.

## Installation Instructions

### 1. Install Dependencies
* Apple.Core

### 2. Install the Package
Run `python3 build.py` from the repository root and import the generated `.tgz` package from the `Build/` directory into your Unity project.

## Requirements
- **Unity**: 2022.3 or later
- **Apple.Core**: 3.1.8 or later (dependency)
- **Platform Support**:
  - iOS 15.0+
  - macOS 12.0+
  - tvOS 15.0+
  - visionOS 1.0+

## Features
- **Product Management**: Fetch and display product information from the App Store
- **In-App Purchases**: Support for consumable, non-consumable, auto-renewable, and non-renewable subscriptions
- **Transaction Handling**: Retrieve, verify, and manage purchase transactions
- **Async/Await Support**: Modern async programming model with `Task` and `IAsyncEnumerable`
- **Transaction Verification**: Built-in support for verifying transaction authenticity
- **App Transactions**: Retrieve and validate app installation transactions

## Usage

Since most calls are asynchronous, public methods are `Task` or `Task<>` based.

### Fetching Products
```csharp
using Apple.StoreKit;
using System.Collections.Generic;

// Define your product IDs
string[] productIds = new string[]
{
    "com.yourcompany.consumable",
    "com.yourcompany.nonconsumable",
    "com.yourcompany.subscription"
};

// Fetch products from the App Store
var products = await Product.FetchProducts(productIds);

foreach (var product in products)
{
    Debug.Log($"Product: {product.DisplayName}");
    Debug.Log($"Price: {product.DisplayPrice}");
    Debug.Log($"Description: {product.Description}");
}
```

### Making a Purchase
```csharp
// Purchase a product
var result = await product.Purchase();

if (result.Result == PurchaseResult.Success)
{
    var transaction = result.VerificationResult.Transaction;
    Debug.Log($"Purchase successful! Transaction ID: {transaction.Id}");

    // Finish the transaction
    await transaction.Finish();
}
else if (result.Result == PurchaseResult.UserCancelled)
{
    Debug.Log("User cancelled the purchase");
}
```

### Purchase Options
You can provide additional options when making a purchase:

```csharp
// Purchase with options
var options = new PurchaseOption[]
{
    PurchaseOption.AppAccountToken("user-uuid-string"),
    PurchaseOption.Quantity(2)
};

var result = await product.Purchase(options);
```

### Retrieving Transactions
```csharp
// Get all transactions for the current user
await foreach (var transaction in Transaction.GetAll())
{
    Debug.Log($"Transaction ID: {transaction.Id}");
    Debug.Log($"Product ID: {transaction.ProductId}");
    Debug.Log($"Purchase Date: {transaction.PurchaseDate}");

    // Process the transaction
    // ...
}
```

### Listening for Transaction Updates
```csharp
// Listen for new transactions (purchases from other devices, renewals, etc.)
await foreach (var verificationResult in Transaction.Updates())
{
    if (verificationResult.IsVerified)
    {
        var transaction = verificationResult.Transaction;
        Debug.Log($"New transaction: {transaction.ProductId}");

        // Deliver content to user
        // ...

        // Finish the transaction
        await transaction.Finish();
    }
}
```

## Best Practices

### Transaction Verification
Always verify transactions before delivering content:
```csharp
var result = await product.Purchase();
if (result.VerificationResult.IsVerified)
{
    // Transaction is authentic, deliver content
    var transaction = result.VerificationResult.Transaction;
    DeliverContent(transaction.ProductId);
    await transaction.Finish();
}
```

### Finishing Transactions
Always finish transactions after processing:
```csharp
await transaction.Finish();
```
Unfinished transactions will be re-delivered on app launch.

### Handling Updates
Set up transaction update listeners early in your app lifecycle:
```csharp
async void Start()
{
    await foreach (var result in Transaction.Updates())
    {
        ProcessTransaction(result.Transaction);
    }
}
```

### Exceptions
If there is any error from StoreKit, it throws `StoreKitException`.

Wrap StoreKit calls in try-catch blocks:
```csharp
try
{
    var products = await Product.FetchProducts(productIds);
}
catch (StoreKitException ex)
{
    Debug.LogError($"StoreKit error: {ex.Message}");
}
```

## Testing

### Xcode StoreKit Configuration
A sample StoreKit configuration file is included at `Demos/Apple.StoreKit.Sample/Sample.storekit`. To use it when running on device or in the Xcode Simulator:

1. Build your Unity project for iOS/macOS to generate the Xcode project.
2. Open the generated `.xcodeproj` in Xcode.
3. In the menu bar choose **Product → Scheme → Edit Scheme…**
4. Select the **Run** action in the left panel.
5. Open the **Options** tab.
6. Set **StoreKit Configuration** to `Sample.storekit` (use the file picker to locate it in the project).
7. Close the scheme editor and run the app.

Purchases will be processed against the local configuration file rather than the live App Store.

### Unity Editor Testing
The sample project includes test buttons that allow you to:
- Add mock products to the product list
- Add mock transactions to the transaction history
- Test your UI layout and flow without building to device

## Sample Project
The plug-in includes a sample scene demonstrating:
- Product list display with the "Refresh Products" button
- Transaction history with the "Reload" button
- Test functionality for development without building to Xcode
- Proper async/await patterns

To explore the sample:
1. Open `Assets/Apple.StoreKit/Demos/Apple.StoreKit.Sample/Scenes/Apple.StoreKit.Sample.Scene.unity`
2. Enter Play mode to test product and transaction UI
3. Use the "Add" buttons to test with mock data in the Unity Editor

## Additional Resources
- [StoreKit 2 Documentation](https://developer.apple.com/documentation/storekit)
- [In-App Purchase Best Practices](https://developer.apple.com/app-store/in-app-purchase/)
- [Testing In-App Purchases](https://developer.apple.com/documentation/storekit/in-app_purchase/testing_in-app_purchases)

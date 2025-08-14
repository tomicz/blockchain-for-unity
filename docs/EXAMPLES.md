# Examples

## Table of Contents

- [Basic Integration](#basic-integration)
- [Wallet Management](#wallet-management)
- [Balance Operations](#balance-operations)
- [Transaction Handling](#transaction-handling)
- [Custom RPC Calls](#custom-rpc-calls)
- [UI Integration](#ui-integration)
- [Error Handling](#error-handling)
- [Advanced Patterns](#advanced-patterns)

## 🚀 Basic Integration

### Simple Setup

```csharp
using UnityEngine;
using BlockchainUnity.Managers;

public class SimpleBlockchainExample : MonoBehaviour
{
    [SerializeField] private BlockchainManager blockchainManager;

    void Start()
    {
        // Subscribe to events
        blockchainManager.OnWalletConnected += OnWalletConnected;
        blockchainManager.OnBalanceReceived += OnBalanceReceived;
        blockchainManager.OnError += OnError;
    }

    void OnDestroy()
    {
        // Clean up event subscriptions
        blockchainManager.OnWalletConnected -= OnWalletConnected;
        blockchainManager.OnBalanceReceived -= OnBalanceReceived;
        blockchainManager.OnError -= OnError;
    }

    public void ConnectToWallet()
    {
        blockchainManager.ConnectWallet();
    }

    private void OnWalletConnected(WalletConnectionResult result)
    {
        Debug.Log($"Connected to wallet: {result.address}");
    }

    private void OnBalanceReceived(BalanceResult result)
    {
        Debug.Log($"Balance: {result.formattedBalance} {result.currencySymbol}");
    }

    private void OnError(string error)
    {
        Debug.LogError($"Blockchain error: {error}");
    }
}
```

## 🎨 UI Integration

### Basic UI Manager

```csharp
public class BlockchainUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMPro.TextMeshProUGUI addressText;
    [SerializeField] private TMPro.TextMeshProUGUI balanceText;
    [SerializeField] private TMPro.TextMeshProUGUI networkText;
    [SerializeField] private TMPro.TextMeshProUGUI statusText;

    [Header("Buttons")]
    [SerializeField] private UnityEngine.UI.Button connectButton;
    [SerializeField] private UnityEngine.UI.Button disconnectButton;
    [SerializeField] private UnityEngine.UI.Button refreshButton;

    [SerializeField] private BlockchainManager blockchainManager;

    void Start()
    {
        SetupUI();
        SetupEventListeners();
    }

    private void SetupUI()
    {
        // Set initial state
        UpdateConnectionState(false);

        // Setup button listeners
        if (connectButton != null)
            connectButton.onClick.AddListener(ConnectWallet);

        if (disconnectButton != null)
            disconnectButton.onClick.AddListener(DisconnectWallet);

        if (refreshButton != null)
            refreshButton.onClick.AddListener(RefreshBalance);
    }

    private void SetupEventListeners()
    {
        blockchainManager.OnWalletConnected += OnWalletConnected;
        blockchainManager.OnWalletDisconnected += OnWalletDisconnected;
        blockchainManager.OnBalanceReceived += OnBalanceReceived;
        blockchainManager.OnError += OnError;
    }

    private void ConnectWallet()
    {
        UpdateStatus("Connecting to MetaMask...");
        blockchainManager.ConnectWallet();
    }

    private void DisconnectWallet()
    {
        blockchainManager.DisconnectWallet();
    }

    private void RefreshBalance()
    {
        if (blockchainManager.IsConnected)
        {
            UpdateStatus("Refreshing balance...");
            blockchainManager.GetBalance();
        }
    }

    private void OnWalletConnected(WalletConnectionResult result)
    {
        UpdateConnectionState(true);
        UpdateAddress(result.address);
        UpdateNetwork(result.chainId);
        UpdateStatus("Connected to MetaMask");

        // Auto-refresh balance
        blockchainManager.GetBalance();
    }

    private void OnWalletDisconnected(string message)
    {
        UpdateConnectionState(false);
        UpdateAddress("Not Connected");
        UpdateBalance("0 ETH");
        UpdateNetwork("Unknown");
        UpdateStatus("Disconnected");
    }

    private void OnBalanceReceived(BalanceResult result)
    {
        if (result.success)
        {
            UpdateBalance($"{result.formattedBalance} {result.currencySymbol}");
            UpdateStatus("Balance updated");
        }
        else
        {
            UpdateBalance("Error");
            UpdateStatus($"Balance error: {result.error}");
        }
    }

    private void OnError(string error)
    {
        UpdateStatus($"Error: {error}");
    }

    private void UpdateConnectionState(bool isConnected)
    {
        if (connectButton != null)
            connectButton.gameObject.SetActive(!isConnected);

        if (disconnectButton != null)
            disconnectButton.gameObject.SetActive(isConnected);

        if (refreshButton != null)
            refreshButton.interactable = isConnected;
    }

    private void UpdateAddress(string address)
    {
        if (addressText != null)
            addressText.text = $"Address: {address}";
    }

    private void UpdateBalance(string balance)
    {
        if (balanceText != null)
            balanceText.text = $"Balance: {balance}";
    }

    private void UpdateNetwork(string chainId)
    {
        if (networkText != null)
        {
            string networkName = GetNetworkName(chainId);
            networkText.text = $"Network: {networkName}";
        }
    }

    private void UpdateStatus(string status)
    {
        if (statusText != null)
            statusText.text = $"Status: {status}";
    }

    private string GetNetworkName(string chainId)
    {
        switch (chainId)
        {
            case "0x1": return "Ethereum Mainnet";
            case "0xaa36a7": return "Sepolia Testnet";
            case "0x89": return "Polygon";
            case "0x38": return "BSC";
            default: return "Unknown";
        }
    }
}
```

## 🚨 Error Handling

### Comprehensive Error Handler

```csharp
public class ErrorHandler : MonoBehaviour
{
    [SerializeField] private BlockchainManager blockchainManager;

    [Header("Error UI")]
    [SerializeField] private GameObject errorPanel;
    [SerializeField] private TMPro.TextMeshProUGUI errorMessageText;
    [SerializeField] private UnityEngine.UI.Button retryButton;
    [SerializeField] private UnityEngine.UI.Button dismissButton;

    private Dictionary<string, string> errorMessages = new Dictionary<string, string>();

    void Start()
    {
        SetupErrorMessages();
        SetupErrorUI();
        blockchainManager.OnError += HandleError;
    }

    private void SetupErrorMessages()
    {
        errorMessages["MetaMask not available"] = "Please install MetaMask extension";
        errorMessages["User rejected"] = "Connection was rejected by user";
        errorMessages["insufficient"] = "Insufficient funds for transaction";
        errorMessages["network"] = "Network connection error";
        errorMessages["timeout"] = "Request timed out";
    }

    private void SetupErrorUI()
    {
        if (retryButton != null)
            retryButton.onClick.AddListener(RetryLastOperation);

        if (dismissButton != null)
            dismissButton.onClick.AddListener(DismissError);
    }

    private void HandleError(string error)
    {
        Debug.LogError($"Blockchain error: {error}");

        // Get user-friendly message
        string userMessage = GetUserFriendlyMessage(error);

        // Show error UI
        ShowError(userMessage);

        // Log for debugging
        LogErrorForDebugging(error);
    }

    private string GetUserFriendlyMessage(string error)
    {
        foreach (var kvp in errorMessages)
        {
            if (error.Contains(kvp.Key))
            {
                return kvp.Value;
            }
        }

        return "An unexpected error occurred. Please try again.";
    }

    private void ShowError(string message)
    {
        if (errorPanel != null)
            errorPanel.SetActive(true);

        if (errorMessageText != null)
            errorMessageText.text = message;
    }

    private void DismissError()
    {
        if (errorPanel != null)
            errorPanel.SetActive(false);
    }

    private void RetryLastOperation()
    {
        DismissError();
        // Implement retry logic based on last operation
    }

    private void LogErrorForDebugging(string error)
    {
        // Log detailed error information for debugging
        Debug.LogError($"[BLOCKCHAIN DEBUG] Error: {error}");
        Debug.LogError($"[BLOCKCHAIN DEBUG] Time: {System.DateTime.Now}");
        Debug.LogError($"[BLOCKCHAIN DEBUG] Connected: {blockchainManager.IsConnected}");
        Debug.LogError($"[BLOCKCHAIN DEBUG] Address: {blockchainManager.CurrentAddress}");
    }
}
```

## 🔗 Related Documentation

- [User Guide](USER_GUIDE.md) - Step-by-step setup and usage
- [API Reference](API_REFERENCE.md) - Complete API documentation
- [Architecture](ARCHITECTURE.md) - Technical architecture overview
- [Troubleshooting](TROUBLESHOOTING.md) - Common issues and solutions

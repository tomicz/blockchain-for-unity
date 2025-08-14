# User Guide

## 📝 Table of Contents

- [Installation](#installation)
- [Quick Start](#quick-start)
- [Basic Usage](#basic-usage)
- [Configuration](#configuration)
- [Supported Networks](#supported-networks)
- [Best Practices](#best-practices)

## 🚀 Installation

### Prerequisites

- Unity 2019.1 or higher
- WebGL build target
- MetaMask browser extension

### Setup Steps

1. **Import the Plugin**

   - Download the latest release
   - Import the `.unitypackage` into your project
   - Or clone the repository into your `Assets/Plugins/` folder

2. **Add BlockchainManager**

   ```csharp
   // Create a GameObject
   var blockchainObject = new GameObject("BlockchainManager");

   // Add the manager component
   var manager = blockchainObject.AddComponent<BlockchainManager>();
   ```

3. **Configure Network**

   ```csharp
   // Set default network (Sepolia testnet)
   manager.SetDefaultChainId("0xaa36a7");
   ```

## ⚡ Quick Start

### Basic Integration

```csharp
public class MyGameScript : MonoBehaviour
{
    [SerializeField] private BlockchainManager blockchainManager;

    void Start()
    {
        // Subscribe to events
        blockchainManager.OnWalletConnected += OnWalletConnected;
        blockchainManager.OnBalanceReceived += OnBalanceReceived;
        blockchainManager.OnError += OnError;
    }

    void ConnectToWallet()
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

## 🔧 Basic Usage

### Connecting to MetaMask

```csharp
// Connect using default chain
blockchainManager.ConnectWallet();

// Connect to specific network
blockchainManager.ConnectWallet("0x1"); // Ethereum Mainnet
blockchainManager.ConnectWallet("0xaa36a7"); // Sepolia Testnet
```

### Getting Wallet Balance

```csharp
// Get balance of connected wallet
blockchainManager.GetBalance();

// Get balance of specific address
blockchainManager.GetBalance("0x1234...");
```

### Sending Transactions

```csharp
// Send ETH to another address
blockchainManager.SendTransaction(
    to: "0x1234...",
    value: "0xde0b6b3a7640000", // 1 ETH in hex
    onSuccess: (response) => Debug.Log("Transaction sent!"),
    onError: (error) => Debug.LogError($"Transaction failed: {error}")
);
```

### Custom RPC Requests

```csharp
var request = new RpcRequest
{
    method = "eth_blockNumber",
    @params = new string[] { },
    id = 1
};

blockchainManager.SendCustomRpcRequest(
    request,
    onSuccess: (response) => Debug.Log($"Block number: {response.result}"),
    onError: (error) => Debug.LogError($"RPC failed: {error}")
);
```

## ⚙️ Configuration

### Network Configuration

Configure your EVM blockchain network through the Unity Editor:

1. **Open Chain Config Editor**

   - Go to **Tomicz Engineering > Chain Config** in the Unity menu
   - This opens the configuration window

2. **Configure Network Settings**

   - **Network Name**: A friendly name for your network (e.g., "sepolia", "ethereum")
   - **Chain ID**: The decimal chain ID (e.g., 11155111 for Sepolia, 1 for Ethereum mainnet)
   - **RPC URL**: Your RPC endpoint (e.g., Infura, Alchemy, or your own node)
   - **Currency Symbol**: The token symbol (e.g., "SepoliaETH", "ETH", "MATIC")
   - **Is Testnet**: Toggle for testnet vs mainnet

3. **Update Configuration**
   - Click **Update Config** to save changes
   - The configuration is saved to `Assets/config.json`

### Example Configurations

#### Sepolia Testnet

```json
{
  "networkName": "sepolia",
  "chainId": 11155111,
  "rpcUrl": "https://sepolia.infura.io/v3/YOUR_PROJECT_ID",
  "currencySymbol": "SepoliaETH",
  "isTestnet": true
}
```

#### Ethereum Mainnet

```json
{
  "networkName": "ethereum",
  "chainId": 1,
  "rpcUrl": "https://mainnet.infura.io/v3/YOUR_PROJECT_ID",
  "currencySymbol": "ETH",
  "isTestnet": false
}
```

#### Polygon

```json
{
  "networkName": "polygon",
  "chainId": 137,
  "rpcUrl": "https://polygon-rpc.com",
  "currencySymbol": "MATIC",
  "isTestnet": false
}
```

### Switching Networks

To switch networks:

1. Open **Tomicz Engineering > Chain Config**
2. Update the network settings
3. Click **Update Config**
4. Restart your Unity application or reload the configuration

### Configuration Validation

The system automatically:

- Converts decimal chain IDs to hexadecimal format (e.g., 11155111 → "0xaa36a7")
- Validates RPC URL format
- Provides fallback values if configuration is missing

## 📄 Supported Networks

| Network          | Chain ID   | Description                                |
| ---------------- | ---------- | ------------------------------------------ |
| Ethereum Mainnet | `0x1`      | Production Ethereum network                |
| Sepolia Testnet  | `0xaa36a7` | Ethereum testnet (recommended for testing) |
| Goerli Testnet   | `0x5`      | Ethereum testnet                           |
| Polygon          | `0x89`     | Polygon mainnet                            |
| Polygon Mumbai   | `0x13881`  | Polygon testnet                            |
| BSC              | `0x38`     | Binance Smart Chain                        |
| BSC Testnet      | `0x61`     | BSC testnet                                |
| Arbitrum One     | `0xa4b1`   | Arbitrum mainnet                           |
| Optimism         | `0xa`      | Optimism mainnet                           |
| Base             | `0x2105`   | Base mainnet                               |

## 💡 Best Practices

### 1. Error Handling

```csharp
blockchainManager.OnError += (error) => {
    // Always handle errors gracefully
    Debug.LogError($"Blockchain error: {error}");

    // Show user-friendly message
    ShowErrorMessage("Connection failed. Please try again.");
};
```

### 2. Connection State Management

```csharp
void Update()
{
    if (blockchainManager.IsConnected)
    {
        // Enable blockchain features
        EnableBlockchainUI();
    }
    else
    {
        // Disable blockchain features
        DisableBlockchainUI();
    }
}
```

### 3. Network Validation

```csharp
void ConnectToWallet()
{
    // Validate network before connecting
    string targetChain = "0xaa36a7"; // Sepolia

    if (IsNetworkSupported(targetChain))
    {
        blockchainManager.ConnectWallet(targetChain);
    }
    else
    {
        Debug.LogError("Network not supported");
    }
}
```

### 4. User Experience

```csharp
void ConnectToWallet()
{
    // Show loading state
    ShowLoadingUI("Connecting to MetaMask...");

    blockchainManager.ConnectWallet();
}

private void OnWalletConnected(WalletConnectionResult result)
{
    // Hide loading state
    HideLoadingUI();

    // Show success message
    ShowSuccessMessage("Connected successfully!");
}
```

## 🔍 Troubleshooting

### Common Issues

- **MetaMask not found**: Ensure MetaMask is installed and unlocked
- **Wrong network**: Make sure you're on the correct network in MetaMask
- **Connection failed**: Check browser console for detailed error messages
- **Balance not showing**: Verify the address has funds on the selected network

### Debug Mode

Enable debug logging to see detailed information:

```csharp
// Add this to your script for debugging
void OnEnable()
{
    Application.logMessageReceived += HandleLog;
}

void HandleLog(string logString, string stackTrace, LogType type)
{
    if (logString.Contains("Blockchain"))
    {
        Debug.Log($"[BLOCKCHAIN DEBUG] {logString}");
    }
}
```

## 📞 Support

If you encounter issues:

1. Check the [Troubleshooting Guide](TROUBLESHOOTING.md)
2. Search [GitHub Issues](https://github.com/tomicz/blockchain-for-unity/issues)
3. Create a new issue with detailed information
4. Join our [Discord Community](https://discord.gg/your-community)

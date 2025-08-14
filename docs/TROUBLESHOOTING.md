# Troubleshooting

## Table of Contents
- [Common Issues](#common-issues)
- [Connection Problems](#connection-problems)
- [Balance Issues](#balance-issues)
- [Transaction Problems](#transaction-problems)
- [Performance Issues](#performance-issues)
- [Debug Tools](#debug-tools)
- [Getting Help](#getting-help)

## �� Common Issues

### MetaMask Not Found
**Problem**: "MetaMask is not available" error

**Solutions**:
1. **Install MetaMask**: Download from [metamask.io](https://metamask.io)
2. **Check Browser**: Ensure you're using a supported browser (Chrome, Firefox, Edge)
3. **Enable Extension**: Make sure MetaMask is enabled in your browser
4. **Refresh Page**: Reload the Unity WebGL build

**Debug Steps**:
```javascript
// Check in browser console
console.log(typeof window.ethereum);
console.log(window.ethereum?.isMetaMask);
```

### Wrong Network
**Problem**: Connected to wrong blockchain network

**Solutions**:
1. **Switch Network**: Use MetaMask to switch to the correct network
2. **Add Network**: Add the network if it's not in MetaMask
3. **Check Chain ID**: Verify the chain ID in your code matches MetaMask

**Code Check**:
```csharp
// Verify your chain ID
string chainId = blockchainManager.GetDefaultChainId();
Debug.Log($"Expected chain ID: {chainId}");
```

### Connection Rejected
**Problem**: User rejected connection request

**Solutions**:
1. **Check MetaMask**: Ensure MetaMask is unlocked
2. **Clear Permissions**: Reset MetaMask permissions for the site
3. **Try Again**: Click connect button again

## 🔌 Connection Problems

### Cannot Connect to MetaMask
**Symptoms**:
- No MetaMask popup appears
- Connection fails immediately
- "MetaMask not available" error

**Diagnosis**:
```csharp
// Add this debug code
void DebugMetaMaskAvailability()
{
    try
    {
        bool isAvailable = blockchainManager.IsConnected;
        Debug.Log($"MetaMask available: {isAvailable}");
    }
    catch (System.Exception e)
    {
        Debug.LogError($"MetaMask check failed: {e.Message}");
    }
}
```

**Solutions**:
1. **Browser Check**: Ensure you're using a WebGL build in a browser
2. **Extension Status**: Verify MetaMask is installed and enabled
3. **Site Permissions**: Check if the site is blocked in MetaMask
4. **Network Issues**: Ensure stable internet connection

### Connection Timeout
**Symptoms**:
- Connection hangs indefinitely
- No response from MetaMask
- Browser becomes unresponsive

**Solutions**:
1. **Refresh Page**: Reload the Unity WebGL build
2. **Clear Cache**: Clear browser cache and cookies
3. **Restart Browser**: Close and reopen the browser
4. **Check MetaMask**: Restart MetaMask extension

### Multiple Connection Attempts
**Problem**: Multiple connection popups or errors

**Solutions**:
```csharp
// Add connection state management
private bool isConnecting = false;

public void ConnectToWallet()
{
    if (isConnecting || blockchainManager.IsConnected)
        return;
    
    isConnecting = true;
    blockchainManager.ConnectWallet();
}

private void OnWalletConnected(WalletConnectionResult result)
{
    isConnecting = false;
    // Handle successful connection
}

private void OnError(string error)
{
    isConnecting = false;
    // Handle error
}
```

## Balance Issues

### Balance Not Showing
**Symptoms**:
- Balance shows as 0 when wallet has funds
- Balance request fails
- No balance update after connection

**Debug Steps**:
```csharp
// Add detailed balance debugging
private void DebugBalanceRequest()
{
    Debug.Log($"Current address: {blockchainManager.CurrentAddress}");
    Debug.Log($"Is connected: {blockchainManager.IsConnected}");
    Debug.Log($"Current chain: {blockchainManager.CurrentChainId}");
    
    // Check if address has funds on the correct network
    blockchainManager.GetBalance();
}
```

**Solutions**:
1. **Check Network**: Ensure you're on the correct network
2. **Verify Address**: Check if the address has funds on the selected network
3. **Test Network**: Use a testnet with known funds
4. **RPC Issues**: Check if the RPC endpoint is working

### Wrong Balance Display
**Symptoms**:
- Balance shows incorrect amount
- Currency symbol is wrong
- Balance formatting issues

**Solutions**:
```csharp
// Add balance validation
private void ValidateBalance(BalanceResult result)
{
    Debug.Log($"Raw balance: {result.balance}");
    Debug.Log($"Formatted balance: {result.formattedBalance}");
    Debug.Log($"Currency symbol: {result.currencySymbol}");
    
    // Verify the balance makes sense
    if (decimal.TryParse(result.formattedBalance, out decimal balance))
    {
        if (balance < 0)
        {
            Debug.LogError("Negative balance detected!");
        }
    }
}
```

### Balance Not Updating
**Symptoms**:
- Balance doesn't refresh after transactions
- Old balance persists
- No real-time updates

**Solutions**:
```csharp
// Implement automatic balance refresh
private float balanceRefreshInterval = 30f;
private float lastBalanceCheck = 0f;

void Update()
{
    if (blockchainManager.IsConnected && 
        Time.time - lastBalanceCheck > balanceRefreshInterval)
    {
        blockchainManager.GetBalance();
        lastBalanceCheck = Time.time;
    }
}
```

## 💸 Transaction Problems

### Transaction Fails
**Symptoms**:
- Transaction rejected by MetaMask
- Transaction fails with error
- No transaction hash returned

**Common Causes**:
1. **Insufficient Funds**: Not enough balance for transaction + gas
2. **Invalid Address**: Recipient address is malformed
3. **Gas Issues**: Gas limit too low or gas price too high
4. **Network Congestion**: High network traffic

**Debug Code**:
```csharp
// Add transaction validation
public void SendTransactionWithValidation(string to, string value)
{
    // Validate recipient address
    if (!IsValidAddress(to))
    {
        Debug.LogError($"Invalid recipient address: {to}");
        return;
    }
    
    // Check balance before sending
    blockchainManager.GetBalance();
    
    // Send transaction with error handling
    blockchainManager.SendTransaction(
        to: to,
        value: value,
        onSuccess: (response) => Debug.Log($"Transaction sent: {response.result}"),
        onError: (error) => Debug.LogError($"Transaction failed: {error}")
    );
}

private bool IsValidAddress(string address)
{
    return !string.IsNullOrEmpty(address) && 
           address.StartsWith("0x") && 
           address.Length == 42;
}
```

### Gas Estimation Fails
**Problem**: Cannot estimate gas for transaction

**Solutions**:
1. **Check Transaction Data**: Verify transaction parameters
2. **Use Manual Gas**: Set gas limit manually
3. **Test with Simple Transaction**: Try sending ETH without data
4. **Check Network**: Ensure RPC endpoint is working

### Transaction Stuck
**Symptoms**:
- Transaction pending for a long time
- No confirmation received
- Gas price too low

**Solutions**:
1. **Increase Gas Price**: Use higher gas price for faster confirmation
2. **Check Network**: Verify network is not congested
3. **Replace Transaction**: Send new transaction with higher nonce
4. **Wait**: Some networks have slower confirmation times

## ⚡ Performance Issues

### Slow Response Times
**Symptoms**:
- Long delays for balance checks
- Slow transaction processing
- UI becomes unresponsive

**Solutions**:
```csharp
// Implement request throttling
private float requestCooldown = 1f;
private float lastRequestTime = 0f;

public void ThrottledGetBalance()
{
    if (Time.time - lastRequestTime < requestCooldown)
    {
        Debug.Log("Request throttled - please wait");
        return;
    }
    
    lastRequestTime = Time.time;
    blockchainManager.GetBalance();
}
```

### Memory Leaks
**Symptoms**:
- Memory usage increases over time
- Performance degrades with use
- Unity crashes after extended use

**Solutions**:
```csharp
// Proper cleanup in OnDestroy
void OnDestroy()
{
    // Unsubscribe from events
    if (blockchainManager != null)
    {
        blockchainManager.OnWalletConnected -= OnWalletConnected;
        blockchainManager.OnBalanceReceived -= OnBalanceReceived;
        blockchainManager.OnError -= OnError;
    }
    
    // Clear any cached data
    ClearCachedData();
}

private void ClearCachedData()
{
    // Clear any stored data, lists, etc.
}
```

## 🔧 Debug Tools

### Debug Logger
```csharp
public class BlockchainDebugger : MonoBehaviour
{
    [SerializeField] private bool enableDebugLogging = true;
    [SerializeField] private BlockchainManager blockchainManager;
    
    private List<string> debugLog = new List<string>();

    void Start()
    {
        if (enableDebugLogging)
        {
            SetupDebugListeners();
        }
    }

    private void SetupDebugListeners()
    {
        blockchainManager.OnWalletConnected += (result) => 
            LogDebug($"Wallet connected: {result.address}");
        
        blockchainManager.OnBalanceReceived += (result) => 
            LogDebug($"Balance received: {result.formattedBalance}");
        
        blockchainManager.OnError += (error) => 
            LogDebug($"Error: {error}");
    }

    private void LogDebug(string message)
    {
        string timestamp = System.DateTime.Now.ToString("HH:mm:ss");
        string logEntry = $"[{timestamp}] {message}";
        
        debugLog.Add(logEntry);
        Debug.Log($"[BLOCKCHAIN DEBUG] {message}");
        
        // Keep only last 100 entries
        if (debugLog.Count > 100)
        {
            debugLog.RemoveAt(0);
        }
    }

    public List<string> GetDebugLog()
    {
        return new List<string>(debugLog);
    }

    public void ClearDebugLog()
    {
        debugLog.Clear();
    }
}
```

### Network Status Checker
```csharp
public class NetworkStatusChecker : MonoBehaviour
{
    [SerializeField] private BlockchainManager blockchainManager;
    
    public void CheckNetworkStatus()
    {
        Debug.Log("=== Network Status Check ===");
        Debug.Log($"Connected: {blockchainManager.IsConnected}");
        Debug.Log($"Address: {blockchainManager.CurrentAddress}");
        Debug.Log($"Chain ID: {blockchainManager.CurrentChainId}");
        Debug.Log($"Default Chain: {blockchainManager.GetDefaultChainId()}");
        
        // Test RPC connection
        TestRpcConnection();
    }

    private void TestRpcConnection()
    {
        var request = new RpcRequest
        {
            method = "eth_blockNumber",
            @params = new string[] { },
            id = 1
        };

        blockchainManager.SendCustomRpcRequest(
            request,
            onSuccess: (response) => Debug.Log($"RPC Test Success: Block {response.result}"),
            onError: (error) => Debug.LogError($"RPC Test Failed: {error}")
        );
    }
}
```

## 🆘 Getting Help

### Before Asking for Help
1. **Check this guide** for your specific issue
2. **Enable debug logging** and check console output
3. **Test with a simple example** to isolate the problem
4. **Check browser console** for JavaScript errors
5. **Verify MetaMask** is working with other dApps

### Information to Include
When reporting an issue, include:
- **Unity version**
- **Browser and version**
- **MetaMask version**
- **Network you're trying to connect to**
- **Complete error message**
- **Steps to reproduce**
- **Debug log output**

### Support Channels
- [GitHub Issues](https://github.com/tomicz/blockchain-for-unity/issues)
- [Discord Community](https://discord.gg/your-community)
- [Documentation](https://docs.blockchainunity.com)

### Common Error Messages

| Error Message | Cause | Solution |
|---------------|-------|----------|
| "MetaMask is not available" | MetaMask not installed/enabled | Install MetaMask extension |
| "User rejected" | User denied connection | Click connect again |
| "insufficient funds" | Not enough balance | Add funds to wallet |
| "invalid address" | Malformed address | Check address format |
| "network error" | RPC endpoint down | Try different RPC or wait |
| "timeout" | Request took too long | Check internet connection |

## 🔗 Related Documentation

- [User Guide](USER_GUIDE.md) - Step-by-step setup and usage
- [API Reference](API_REFERENCE.md) - Complete API documentation
- [Architecture](ARCHITECTURE.md) - Technical architecture overview
- [Examples](EXAMPLES.md) - Code examples and use cases

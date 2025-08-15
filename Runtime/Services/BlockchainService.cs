using UnityEngine;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.IO; // Add this for Path and File
using BlockchainUnity.Core;
using BlockchainUnity.Models;

namespace BlockchainUnity.Services
{
    /// <summary>
    /// High-level service for blockchain operations
    /// </summary>
    public class BlockchainService : MonoBehaviour
    {
        // Events
        public event Action<WalletConnectionResult> OnWalletConnected;
        public event Action<string> OnWalletDisconnected;
        public event Action<BalanceResult> OnBalanceReceived;
        public event Action<string> OnError;

        // Properties
        public bool IsConnected => !string.IsNullOrEmpty(CurrentAddress);
        public string CurrentAddress { get; private set; }
        public string CurrentChainId { get; private set; }

        // Dependencies (can be injected)
        private IEthereumBridge _ethereumBridge;
        private Dictionary<int, RequestCallback> _pendingRequests = new Dictionary<int, RequestCallback>();
        
        private class RequestCallback
        {
            public Action<RpcResponse> onSuccess;
            public Action<string> onError;
        }

        /// <summary>
        /// Initialize the service with dependencies
        /// </summary>
        public void Initialize(IEthereumBridge ethereumBridge = null)
        {
            _ethereumBridge = ethereumBridge ?? new EthereumWebBridgeWrapper();
        }

        private void Awake()
        {
            // Auto-initialize with default bridge if not set
            if (_ethereumBridge == null)
            {
                Initialize();
            }
        }

        /// <summary>
        /// Connect to MetaMask wallet
        /// </summary>
        public void ConnectWallet()
        {
            NetworkConfig networkConfig = GetNetworkConfig();
            
            if (!_ethereumBridge.CheckMetaMaskAvailability())
            {
                OnError?.Invoke("MetaMask is not available");
                return;
            }

            Debug.Log($"Connecting to network: {networkConfig.DisplayName} (Chain ID: {networkConfig.HexChainId})");
            _ethereumBridge.ConnectWallet(networkConfig.HexChainId, gameObject.name, "OnWalletConnectionSuccess", "OnWalletConnectionError");
        }

        /// <summary>
        /// Disconnect from wallet
        /// </summary>
        public void DisconnectWallet()
        {
            CurrentAddress = null;
            CurrentChainId = null;
            OnWalletDisconnected?.Invoke("Disconnected");
        }

        /// <summary>
        /// Get balance for current address
        /// </summary>
        public void GetBalance(string address = null)
        {
            string targetAddress = address ?? CurrentAddress;
            if (string.IsNullOrEmpty(targetAddress))
            {
                OnError?.Invoke("No address available");
                return;
            }

            RpcRequest request = new RpcRequest
            {
                method = "eth_getBalance",
                @params = new string[] { targetAddress, "latest" },
                id = UnityEngine.Random.Range(1, 1000)
            };

            string jsonRequest = JsonUtility.ToJson(request);
            _ethereumBridge.SendRpcRequest(jsonRequest, gameObject.name, "OnBalanceSuccess", "OnBalanceError");
        }

        /// <summary>
        /// Send custom RPC request
        /// </summary>
        public void SendRpcRequest(RpcRequest request, Action<RpcResponse> onSuccess, Action<string> onError)
        {
            string jsonRequest = JsonUtility.ToJson(request);
            _pendingRequests[request.id] = new RequestCallback { onSuccess = onSuccess, onError = onError };
            _ethereumBridge.SendRpcRequest(jsonRequest, gameObject.name, "OnRpcSuccess", "OnRpcError");
        }

        // Callback methods (called from JavaScript)
        private void OnWalletConnectionSuccess(string address)
        {
            CurrentAddress = address;
            CurrentChainId = "0xaa36a7"; // You might want to get this dynamically
            OnWalletConnected?.Invoke(new WalletConnectionResult 
            { 
                success = true, 
                address = address,
                chainId = CurrentChainId
            });
        }

        private void OnWalletConnectionError(string error)
        {
            OnError?.Invoke($"Wallet connection failed: {error}");
        }

        private void OnBalanceSuccess(string jsonResponse)
        {
            try
            {
                RpcResponse response = JsonUtility.FromJson<RpcResponse>(jsonResponse);
                
                // Check if there's actually an error (not just an empty error object)
                if (response.error != null && !string.IsNullOrEmpty(response.error.message))
                {
                    OnError?.Invoke(response.error.message);
                    return;
                }

                // If we have a result, proceed with parsing
                if (!string.IsNullOrEmpty(response.result))
                {
                    (string formatted, string symbol) balance = ParseBalance(response.result);
                    
                    BalanceResult balanceResult = new BalanceResult
                    {
                        success = true,
                        balance = response.result,
                        formattedBalance = balance.formatted,
                        currencySymbol = balance.symbol
                    };
                    
                    OnBalanceReceived?.Invoke(balanceResult);
                }
                else
                {
                    OnError?.Invoke("No result in RPC response");
                }
            }
            catch (Exception e)
            {
                OnError?.Invoke($"Failed to parse balance: {e.Message}");
            }
        }

        private void OnBalanceError(string error)
        {
            OnError?.Invoke($"Balance request failed: {error}");
        }

        private void OnRpcSuccess(string jsonResponse)
        {
            try
            {
                RpcResponse response = JsonUtility.FromJson<RpcResponse>(jsonResponse);
                if (_pendingRequests.TryGetValue(response.id, out RequestCallback callback))
                {
                    _pendingRequests.Remove(response.id);
                    callback.onSuccess?.Invoke(response);
                }
            }
            catch (Exception e)
            {
                OnError?.Invoke($"Failed to parse RPC response: {e.Message}");
            }
        }

        private void OnRpcError(string jsonResponse)
        {
            try
            {
                RpcResponse response = JsonUtility.FromJson<RpcResponse>(jsonResponse);
                if (_pendingRequests.TryGetValue(response.id, out RequestCallback callback))
                {
                    _pendingRequests.Remove(response.id);
                    callback.onError?.Invoke(response.error?.message ?? "Unknown error");
                }
            }
            catch (Exception e)
            {
                OnError?.Invoke($"Failed to parse RPC error: {e.Message}");
            }
        }

        private (string formatted, string symbol) ParseBalance(string hexBalance)
        {
            try
            {
                NetworkConfig networkConfig = GetNetworkConfig();
                
                string hexWithoutPrefix = hexBalance.Substring(2);
                BigInteger wei = BigInteger.Parse("0" + hexWithoutPrefix, System.Globalization.NumberStyles.HexNumber);
                
                decimal eth = (decimal)wei / (decimal)Math.Pow(10, 18);
                string formatted = eth.ToString("F6").TrimEnd('0').TrimEnd('.');
                if (formatted == "") formatted = "0";
                
                return (formatted, networkConfig.currencySymbol);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error in ParseBalance: {e.Message}");
                return ("0", "ETH");
            }
        }

        private NetworkConfig _config;

        private void LoadConfiguration()
        {
            try
            {
                string configPath = Path.Combine(Application.dataPath, "config.json");
                if (File.Exists(configPath))
                {
                    string jsonContent = File.ReadAllText(configPath);
                    _config = JsonUtility.FromJson<NetworkConfig>(jsonContent);
                    Debug.Log($"Configuration loaded. Network: {_config.DisplayName}");
                }
                else
                {
                    Debug.LogError("config.json not found!");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load configuration: {e.Message}");
            }
        }

        public NetworkConfig GetNetworkConfig()
        {
            if (_config == null) LoadConfiguration();
            
            if (_config != null)
            {
                return _config;
            }
            
            // Fallback to Sepolia
            return new NetworkConfig
            {
                networkName = "sepolia",
                chainId = 11155111,
                rpcUrl = "https://sepolia.infura.io/v3/YOUR_PROJECT_ID",
                currencySymbol = "SepoliaETH",
                isTestnet = true
            };
        }

        // Add method to get current network info
        public NetworkConfig GetCurrentNetwork()
        {
            return GetNetworkConfig();
        }
    }
}

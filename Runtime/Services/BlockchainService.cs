using UnityEngine;
using System;
using System.Collections.Generic;
using System.Numerics;
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
        public void ConnectWallet(string chainId = "0xaa36a7") // Default: Sepolia
        {
            if (!_ethereumBridge.CheckMetaMaskAvailability())
            {
                OnError?.Invoke("MetaMask is not available");
                return;
            }

            _ethereumBridge.ConnectWallet(chainId, gameObject.name, "OnWalletConnectionSuccess", "OnWalletConnectionError");
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

            var request = new RpcRequest
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
                var response = JsonUtility.FromJson<RpcResponse>(jsonResponse);
                if (response.error != null)
                {
                    OnError?.Invoke(response.error.message);
                    return;
                }

                var balance = ParseBalance(response.result);
                OnBalanceReceived?.Invoke(new BalanceResult
                {
                    success = true,
                    balance = response.result,
                    formattedBalance = balance.formatted,
                    currencySymbol = balance.symbol
                });
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
                var response = JsonUtility.FromJson<RpcResponse>(jsonResponse);
                if (_pendingRequests.TryGetValue(response.id, out var callback))
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
                var response = JsonUtility.FromJson<RpcResponse>(jsonResponse);
                if (_pendingRequests.TryGetValue(response.id, out var callback))
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
            BigInteger wei = BigInteger.Parse("0" + hexBalance.Substring(2), System.Globalization.NumberStyles.HexNumber);
            decimal eth = (decimal)wei / (decimal)Math.Pow(10, 18);
            string formatted = eth.ToString("F6").TrimEnd('0').TrimEnd('.');
            if (formatted == "") formatted = "0";
            
            return (formatted, "ETH"); // You can make this dynamic based on chain
        }
    }
}

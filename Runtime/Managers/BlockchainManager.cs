using UnityEngine;
using BlockchainUnity.Services;
using BlockchainUnity.Models;

namespace BlockchainUnity.Managers
{
    /// <summary>
    /// High-level manager for easy blockchain integration
    /// </summary>
    public class BlockchainManager : MonoBehaviour
    {
        // Events
        public System.Action<WalletConnectionResult> OnWalletConnected;
        public System.Action<string> OnWalletDisconnected;
        public System.Action<BalanceResult> OnBalanceReceived;
        public System.Action<string> OnError;

        // Service reference
        private BlockchainService _blockchainService;
        private string _defaultChainId = "0xaa36a7"; // Sepolia

        private void Start()
        {
            SetupService();
            SetupEventListeners();
        }

        private void SetupService()
        {
            // Find existing service or create new one
            _blockchainService = FindObjectOfType<BlockchainService>();
            if (_blockchainService == null)
            {
                var serviceGameObject = new GameObject("BlockchainService");
                _blockchainService = serviceGameObject.AddComponent<BlockchainService>();
            }
        }

        private void SetupEventListeners()
        {
            if (_blockchainService != null)
            {
                _blockchainService.OnWalletConnected += OnWalletConnected;
                _blockchainService.OnWalletDisconnected += OnWalletDisconnected;
                _blockchainService.OnBalanceReceived += OnBalanceReceived;
                _blockchainService.OnError += OnError;
            }
        }

        private void OnDestroy()
        {
            if (_blockchainService != null)
            {
                _blockchainService.OnWalletConnected -= OnWalletConnected;
                _blockchainService.OnWalletDisconnected -= OnWalletDisconnected;
                _blockchainService.OnBalanceReceived -= OnBalanceReceived;
                _blockchainService.OnError -= OnError;
            }
        }

        // Public API methods
        public void ConnectWallet()
        {
            _blockchainService?.ConnectWallet(_defaultChainId);
        }

        public void ConnectWallet(string chainId)
        {
            _blockchainService?.ConnectWallet(chainId);
        }

        public void DisconnectWallet()
        {
            _blockchainService?.DisconnectWallet();
        }

        public void GetBalance()
        {
            _blockchainService?.GetBalance();
        }

        public void GetBalance(string address)
        {
            _blockchainService?.GetBalance(address);
        }

        public void SendTransaction(string to, string value, string data = null, System.Action<RpcResponse> onSuccess = null, System.Action<string> onError = null)
        {
            var request = new RpcRequest
            {
                method = "eth_sendTransaction",
                @params = new string[] 
                { 
                    JsonUtility.ToJson(new TransactionRequest 
                    { 
                        to = to, 
                        value = value, 
                        data = data ?? "0x" 
                    }) 
                },
                id = Random.Range(1, 1000)
            };

            _blockchainService?.SendRpcRequest(request, onSuccess, onError);
        }

        public void SendCustomRpcRequest(RpcRequest request, System.Action<RpcResponse> onSuccess, System.Action<string> onError)
        {
            _blockchainService?.SendRpcRequest(request, onSuccess, onError);
        }

        // Properties
        public bool IsConnected => _blockchainService?.IsConnected ?? false;
        public string CurrentAddress => _blockchainService?.CurrentAddress;
        public string CurrentChainId => _blockchainService?.CurrentChainId;

        // Configuration methods
        public void SetDefaultChainId(string chainId)
        {
            _defaultChainId = chainId;
        }

        public string GetDefaultChainId()
        {
            return _defaultChainId;
        }
    }
}

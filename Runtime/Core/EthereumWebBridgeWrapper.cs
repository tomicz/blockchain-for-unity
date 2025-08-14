using UnityEngine;

namespace BlockchainUnity.Core
{
    /// <summary>
    /// Wrapper for EthereumWebBridge to implement IEthereumBridge interface
    /// </summary>
    public class EthereumWebBridgeWrapper : IEthereumBridge
    {
        public bool CheckMetaMaskAvailability() => EthereumWebBridge.CheckMetaMaskAvailability();
        public void ConnectWallet(string chainId, string gameObjectName, string callback, string fallback) 
            => EthereumWebBridge.ConnectWallet(chainId, gameObjectName, callback, fallback);
        public void SendRpcRequest(string rpcRequest, string gameObject, string callback, string error) 
            => EthereumWebBridge.SendRpcRequest(rpcRequest, gameObject, callback, error);
    }
}

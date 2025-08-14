namespace BlockchainUnity.Core
{
    /// <summary>
    /// Interface for Ethereum bridge operations
    /// </summary>
    public interface IEthereumBridge
    {
        bool CheckMetaMaskAvailability();
        void ConnectWallet(string chainId, string gameObjectName, string callback, string fallback);
        void SendRpcRequest(string rpcRequest, string gameObject, string callback, string error);
    }
}




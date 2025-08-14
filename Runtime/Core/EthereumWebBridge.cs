using System.Runtime.InteropServices;
using UnityEngine;

namespace BlockchainUnity.Core
{
    /// <summary>
    /// Low-level bridge to JavaScript functions. This should rarely be used directly.
    /// </summary>
    public static class EthereumWebBridge
    {
        [DllImport("__Internal")]
        private static extern bool IsMetaMask();

        [DllImport("__Internal")]
        private static extern void Connect(string chainId, string gameObjectName, string callback, string fallback);

        [DllImport("__Internal")]
        private static extern void CallRpc(string rpcRequest, string gameObject, string callback, string error);

        #if UNITY_WEBGL && !UNITY_EDITOR
        public static bool CheckMetaMaskAvailability() => IsMetaMask();
        public static void ConnectWallet(string chainId, string gameObjectName, string callback, string fallback) 
            => Connect(chainId, gameObjectName, callback, fallback);
        public static void SendRpcRequest(string rpcRequest, string gameObject, string callback, string error) 
            => CallRpc(rpcRequest, gameObject, callback, error);
        #else
        public static bool CheckMetaMaskAvailability() => throw new System.PlatformNotSupportedException("MetaMask is only available in WebGL builds");
        public static void ConnectWallet(string chainId, string gameObjectName, string callback, string fallback) 
            => throw new System.PlatformNotSupportedException("MetaMask is only available in WebGL builds");
        public static void SendRpcRequest(string rpcRequest, string gameObject, string callback, string error) 
            => throw new System.PlatformNotSupportedException("MetaMask is only available in WebGL builds");
        #endif
    }
}

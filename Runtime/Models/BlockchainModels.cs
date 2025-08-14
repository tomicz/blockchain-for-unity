using System;
using System.Collections.Generic;

namespace BlockchainUnity.Models
{
    [Serializable]
    public class RpcRequest
    {
        public string jsonrpc = "2.0";
        public string method;
        public string[] @params;
        public int id;
    }

    [Serializable]
    public class RpcResponse
    {
        public string jsonrpc;
        public string result;
        public int id;
        public RpcError error;
    }

    [Serializable]
    public class RpcError
    {
        public string message;
        public int code;
        
        // Helper property to check if this is a real error
        public bool HasError => !string.IsNullOrEmpty(message);
    }

    [Serializable]
    public class WalletConnectionResult
    {
        public bool success;
        public string address;
        public string error;
        public string chainId;
    }

    [Serializable]
    public class BalanceResult
    {
        public bool success;
        public string balance;
        public string formattedBalance;
        public string currencySymbol;
        public string error;
    }

    [Serializable]
    public class TransactionRequest
    {
        public string to;
        public string value;
        public string data;
    }

    [System.Serializable]
    public class NetworkConfig
    {
        public string networkName = "sepolia";
        public int chainId = 11155111;
        public string rpcUrl = "https://sepolia.infura.io/v3/YOUR_PROJECT_ID";
        public string currencySymbol = "SepoliaETH";
        public bool isTestnet = true;
        
        // Helper property to get hex chain ID
        public string HexChainId => "0x" + chainId.ToString("x");
        
        // Helper property to get display name
        public string DisplayName => $"{networkName} ({currencySymbol})";
    }
}

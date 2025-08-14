using System;

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
}

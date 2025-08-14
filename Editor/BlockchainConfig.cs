using System;
using System.Collections.Generic;

[System.Serializable]
public class BlockchainConfig
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

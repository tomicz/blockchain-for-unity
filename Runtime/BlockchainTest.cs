using UnityEngine;
using Nethereum.Web3;
using System.Threading.Tasks;
using TMPro;
using System.IO;

[System.Serializable]
public class BlockchainConfig
{
    public string RpcUrl;
}

public class BlockchainTest : MonoBehaviour
{
    private Web3 web3;
    private bool isConnected = false;
    private BlockchainConfig config;
    
    private void Start()
    {
        LoadConfig();
        TestBlockchainConnection();
    }
    
    private void LoadConfig()
    {
        try
        {
            string configPath = Path.Combine(Application.dataPath, "config.json");
            if (File.Exists(configPath))
            {
                string jsonContent = File.ReadAllText(configPath);
                config = JsonUtility.FromJson<BlockchainConfig>(jsonContent);
                Debug.Log($"Config loaded successfully. RPC URL: {config.RpcUrl}");
            }
            else
            {
                Debug.LogError("config.json file not found in Assets folder!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load config: {e.Message}");
        }
    }
    
    public async void TestBlockchainConnection()
    {
        if (config == null)
        {
            Debug.LogError("Config not loaded. Cannot test blockchain connection.");
            return;
        }
        
        try
        {
            // Initialize Web3 with RPC URL from config
            web3 = new Web3(config.RpcUrl);
            
            // Test 1: Get current block number
            var blockNumber = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
            
            // Test 2: Get gas price
            var gasPrice = await web3.Eth.GasPrice.SendRequestAsync();
            
            // Test 3: Get network ID
            var networkId = await web3.Net.Version.SendRequestAsync();
            
            isConnected = true;
            
            Debug.Log($"Blockchain connection successful!");
            Debug.Log($"Network ID: {networkId}");
            Debug.Log($"Current Block: {blockNumber.Value}");
            Debug.Log($"Gas Price: {gasPrice.Value} Wei");
            
        }
        catch (System.Exception e)
        {
            isConnected = false;
            
            Debug.Log("Connection failed");
            
            Debug.LogError($"Blockchain connection failed: {e.Message}");
        }
    }
    
    public bool IsConnected()
    {
        return isConnected;
    }
    
    // Optional: Test with a specific Ethereum address
    public async void TestGetBalance(string address)
    {
        if (!isConnected || web3 == null)
        {
            Debug.LogWarning("Not connected to blockchain. Run TestBlockchainConnection first.");
            return;
        }
        
        try
        {
            var balance = await web3.Eth.GetBalance.SendRequestAsync(address);
            Debug.Log($"Balance of {address}: {balance.Value} Wei");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error getting balance: {e.Message}");
        }
    }
}

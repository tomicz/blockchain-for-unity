using UnityEngine;
using UnityEditor;
using System.IO;

public class ChainConfigEditor : EditorWindow
{
    private static BlockchainConfig config;
    private static string configPath;

    [MenuItem("Tomicz Engineering/Chain Config")]
    private static void OpenConfig(){
        LoadConfig();
        ChainConfigEditor window = GetWindow<ChainConfigEditor>("Chain Config");
        window.minSize = new Vector2(400, 300);
    }

    private void OnGUI(){
        if (config == null) return;

        GUILayout.Space(10);
        GUILayout.Label("Network Configuration", EditorStyles.boldLabel);

        // Network name field
        GUILayout.Space(10);
        config.networkName = EditorGUILayout.TextField("Network Name", config.networkName);

        // Chain ID field
        GUILayout.Space(10);
        config.chainId = EditorGUILayout.IntField("Chain ID (Decimal)", config.chainId);
        EditorGUILayout.LabelField("Hex Chain ID", config.HexChainId);

        // RPC URL field
        GUILayout.Space(10);
        config.rpcUrl = EditorGUILayout.TextField("RPC URL", config.rpcUrl);

        // Currency symbol field
        GUILayout.Space(10);
        config.currencySymbol = EditorGUILayout.TextField("Currency Symbol", config.currencySymbol);

        // Is testnet toggle
        GUILayout.Space(10);
        config.isTestnet = EditorGUILayout.Toggle("Is Testnet", config.isTestnet);

        // Network info display
        GUILayout.Space(20);
        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("Current Configuration:", EditorStyles.boldLabel);
        GUILayout.Label($"Network: {config.DisplayName}");
        GUILayout.Label($"Chain ID: {config.HexChainId}");
        GUILayout.Label($"RPC URL: {config.rpcUrl}");
        GUILayout.Label($"Type: {(config.isTestnet ? "Testnet" : "Mainnet")}");
        EditorGUILayout.EndVertical();

        // Update config button
        GUILayout.Space(20);
        if (GUILayout.Button("Update Config", GUILayout.Height(30)))
        {
            UpdateConfig();
            Debug.Log("Config updated successfully!");
        }
    }

    private static void LoadConfig(){
        configPath = Path.Combine(Application.dataPath, "config.json");

        if (File.Exists(configPath))
        {
            string jsonContent = File.ReadAllText(configPath);
            config = JsonUtility.FromJson<BlockchainConfig>(jsonContent);
            Debug.Log($"Config loaded successfully. Network: {config.DisplayName}");
        }
        else
        {
            Debug.LogWarning("config.json not found — creating new one with Sepolia testnet.");

            config = new BlockchainConfig
            {
                networkName = "sepolia",
                chainId = 11155111,
                rpcUrl = "https://sepolia.infura.io/v3/YOUR_PROJECT_ID",
                currencySymbol = "SepoliaETH",
                isTestnet = true
            };

            string jsonContent = JsonUtility.ToJson(config, true);
            File.WriteAllText(configPath, jsonContent);

            AssetDatabase.Refresh();
            Debug.Log($"New config.json created at: {configPath}");
        }
    }

    private void UpdateConfig(){
        string jsonContent = JsonUtility.ToJson(config, true);
        File.WriteAllText(configPath, jsonContent);
        AssetDatabase.Refresh();
    }
}

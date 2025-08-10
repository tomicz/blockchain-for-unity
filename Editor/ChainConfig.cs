using UnityEngine;
using UnityEditor;
using System.IO;

public class ChainConfig : EditorWindow
{
    private static BlockchainConfig config;
    private static string configPath;

    [MenuItem("Blockchain/Config")]
    private static void OpenConfig(){
        LoadConfig();
        ChainConfig window = GetWindow<ChainConfig>("Chain Config");
    }

    private void OnGUI(){
        GUILayout.Space(10);
        config.RpcUrl = EditorGUILayout.TextField("Rpc Url", config.RpcUrl);

        // Update config button
        
        GUILayout.Space(10);

        if(GUILayout.Button("Update config", GUILayout.Height(30))){
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
            Debug.Log($"Config loaded successfully. RPC URL: {config.RpcUrl}");
        }
        else
        {
            Debug.LogWarning("config.json not found — creating new one in Assets/ with placeholder RpcUrl.");

            config = new BlockchainConfig
            {
                RpcUrl = "https://your-placeholder-url.com"
            };

            string jsonContent = JsonUtility.ToJson(config, true);
            File.WriteAllText(configPath, jsonContent);

            AssetDatabase.Refresh();

            string reloadedContent = File.ReadAllText(configPath);
            config = JsonUtility.FromJson<BlockchainConfig>(reloadedContent);

            Debug.Log($"New config.json created at: {configPath}");
        }
    }

    private void UpdateConfig(){
        string jsonContent = JsonUtility.ToJson(config, true);
        File.WriteAllText(configPath, jsonContent);
    }
}

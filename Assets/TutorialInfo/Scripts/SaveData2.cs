using UnityEngine;
using System.IO;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int level;
    public long score;
}

public class JsonSaveSystem : MonoBehaviour
{
    private string filePath;
    
    void Start()
    {
        filePath = Application.persistentDataPath + "/playerdata.json";
    }
    
    public void SaveGame()
    {
        PlayerData data = new PlayerData
        {
            playerName = "Thiago",
            level = 0,
            score = 0
        };
        
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
        
        Debug.Log("Jogo salvo em: " + filePath);
    }
    
    public void LoadGame()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            
            Debug.Log($"Dados carregados - Nome: {data.playerName}, Nível: {data.level}");
        }
        else
        {
            Debug.LogWarning("Arquivo de save não encontrado!");
        }
    }
}
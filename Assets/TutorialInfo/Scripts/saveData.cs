using UnityEngine;

public class PlayerPrefsExample : MonoBehaviour
{
    void Start()
    {
        // Salvar dados
        PlayerPrefs.SetString("PlayerName", "Thiago");
        PlayerPrefs.SetInt("HighScore", 1000);
        PlayerPrefs.SetFloat("Volume", 0.8f);
        PlayerPrefs.Save(); // Força a gravação imediata
        
        // Carregar dados
        string playerName = PlayerPrefs.GetString("PlayerName", "Default");
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        float volume = PlayerPrefs.GetFloat("Volume", 1.0f);
        
        Debug.Log($"Nome: {playerName}, Pontuação: {highScore}, Volume: {volume}");
    }
    
    void OnApplicationQuit()
    {
        // Limpar todos os dados (opcional)
        // PlayerPrefs.DeleteAll();
    }
}
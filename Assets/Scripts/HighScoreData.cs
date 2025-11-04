using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class HighScoreData
{
    public List<HighScoreEntry> highScores = new List<HighScoreEntry>();
    private const string FileName = "highscores.json";

    public static HighScoreData Load()
    {
        string filePath = Path.Combine(Application.persistentDataPath, FileName);
        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                HighScoreData data = JsonUtility.FromJson<HighScoreData>(json);
                // Garante que a lista está ordenada ao carregar
                data.highScores.Sort();
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError("Erro ao carregar HighScores: " + e.Message);
                return new HighScoreData();
            }
        }
        return new HighScoreData();
    }

    public void Save()
    {
        string filePath = Path.Combine(Application.persistentDataPath, FileName);
        try
        {
            // Garante que a lista está ordenada antes de salvar
            highScores.Sort();
            string json = JsonUtility.ToJson(this, true);
            File.WriteAllText(filePath, json);
            Debug.Log("HighScores salvos em: " + filePath);
        }
        catch (Exception e)
        {
            Debug.LogError("Erro ao salvar HighScores: " + e.Message);
        }
    }

    public void AddScore(string playerName, int score, int level)
    {
        HighScoreEntry newEntry = new HighScoreEntry(playerName, score, level);
        highScores.Add(newEntry);
        // Mantém apenas as 10 melhores pontuações (ou o número que você preferir)
        highScores.Sort();
        if (highScores.Count > 10)
        {
            highScores.RemoveRange(10, highScores.Count - 10);
        }
        Save();
    }
}

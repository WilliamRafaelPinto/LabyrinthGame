using UnityEngine;
using TMPro;
using System.Text;

public class HighScoreDisplay : MonoBehaviour
{
    public TextMeshProUGUI highScoreText;
    public int maxEntries = 10;

    void Start()
    {
        DisplayHighScores();
    }

    public void DisplayHighScores()
    {
        if (highScoreText == null)
        {
            Debug.LogError("TextMeshProUGUI para HighScore não atribuído.");
            return;
        }

        HighScoreData data = HighScoreData.Load();
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("--- RANKING ---");
        //sb.AppendLine("Pos. | Nome | Pontuação | Nível");
        //sb.AppendLine("---------------------------------");

        for (int i = 0; i < Mathf.Min(maxEntries, data.highScores.Count); i++)
        {
            HighScoreEntry entry = data.highScores[i];
            sb.AppendLine($"{i + 1:00}. | {entry.playerName.PadRight(10).Substring(0, Mathf.Min(10, entry.playerName.Length))} | {entry.score:00000} | {entry.level}");
        }

        highScoreText.text = sb.ToString();
    }
}

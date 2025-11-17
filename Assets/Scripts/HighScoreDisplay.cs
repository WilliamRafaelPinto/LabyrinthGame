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
        sb.Append("<mspace=0.6em>");
        //sb.AppendLine("--- RANKING ---");
        //sb.AppendLine("N# | name.............. | score | Lv");
        //sb.AppendLine("---------------------------------");

        for (int i = 0; i < Mathf.Min(maxEntries, data.highScores.Count); i++)
    {
        HighScoreEntry entry = data.highScores[i];
        
        // 1. Format the fixed-width number and level
        string rank = $"{i + 1:00}";
        string score = $"{entry.score:00000}";
        string level = $"{entry.level:00}";

        // 2. Create the flexible middle part (Name + Dots)
        string nameField = entry.playerName.Length > 10 ? entry.playerName.Substring(0, 10) : entry.playerName;
        int targetTotalWidth = 30; // Adjust this value until the lines look even
        int currentLineLength = rank.Length + nameField.Length + score.Length + level.Length + 3; // +3 for the " | " separators
        int dotsToAdd = targetTotalWidth - currentLineLength;

        string dots = dotsToAdd > 0 ? new string('.', dotsToAdd) : "";

        // 3. Assemble the final line
        sb.AppendLine($"{rank} | {nameField}{dots} | {score} | {level}");
    }
        sb.Append("</mspace>");
        highScoreText.text = sb.ToString();
    }
}

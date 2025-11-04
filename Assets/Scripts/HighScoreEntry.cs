using System;

[Serializable]
public class HighScoreEntry : IComparable<HighScoreEntry>
{
    public string playerName;
    public int score;
    public int level;

    public HighScoreEntry(string name, int s, int l)
    {
        playerName = name;
        score = s;
        level = l;
    }

    // Permite a comparação para ordenação (maior pontuação primeiro)
    public int CompareTo(HighScoreEntry other)
    {
        // Ordem decrescente de pontuação
        int scoreComparison = other.score.CompareTo(this.score);
        if (scoreComparison != 0)
        {
            return scoreComparison;
        }
        // Em caso de empate, ordem decrescente de nível
        return other.level.CompareTo(this.level);
    }
}

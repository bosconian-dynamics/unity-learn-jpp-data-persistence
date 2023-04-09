using System;

[Serializable]
public class ScoreRecord
{
    public string name;
    public int score;

    public ScoreRecord(string name, int score)
    {
        this.name = name;
        this.score = score;
    }
}
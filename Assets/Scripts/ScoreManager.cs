using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeReference] private ScoreRecord[] Highscores;
    [SerializeField] private string _saveFileName = "save.json";
    [SerializeField] private int _trackedScores = 5;

    private string _saveFilePath => $"{Application.persistentDataPath}/{_saveFileName}";

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Highscores = new ScoreRecord[_trackedScores];
        EventManager.Instance.SetHighScores(Highscores);
        loadHighscores();

        EventManager.Instance.OnGameOver += handleGameOver;
        EventManager.Instance.OnHighScoreSubmitted += setHighScore;
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnGameOver -= handleGameOver;
        EventManager.Instance.OnHighScoreSubmitted -= setHighScore;
    }

    private void handleGameOver(int finalScore)
    {
        int highscoreIndex = getScoreIndex(finalScore);

        EventManager.Instance.RankScore(finalScore, highscoreIndex + 1);
    }

    private void loadHighscores()
    {
        if (!File.Exists(_saveFilePath))
            return;

        string json = File.ReadAllText(_saveFilePath);
        ScoreRecord[] scores = JsonUtility.FromJson<ScoreRecordWrapper>(json).highscores;

        // Transfer scores individually, in case more were persisted than are presently set to be tracked.
        for(int i = 0; i < scores.Length && i < Highscores.Length && scores[i].name != ""; i++)
            Highscores[i] = scores[i];

        EventManager.Instance.SetHighScores(Highscores);
    }

    private void saveHighscores()
    {
        if (Highscores[0] == null)
            return;

        ScoreRecordWrapper data = new ScoreRecordWrapper();
        data.highscores = Highscores.Where(scoreRecord => scoreRecord != null && scoreRecord.name != "").ToArray();
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(_saveFilePath, json);
    }

    // Determines the index within the Highscores array where a given score would be placed.
    private int getScoreIndex(int score)
    {
        if (score == 0)
            return -1;

        for (int i = 0; i < Highscores.Length; i++)
        {
            // If we've iterated to an empty slot, it would place here.
            if (Highscores[i] == null)
                return i;

            // If the score in this slot is larger than the submitted score, immediately advance to the next slot.
            if (Highscores[i].score > score) continue;

            // At this point, the current slot is smaller than the submitted score - this is where it would place.
            return i;
        }

        return -1; // Score would not place on leaderboard.
    }

    private void setHighScore(int score, string name, int? rank)
    {
        int i = rank.HasValue ? rank.Value - 1 : getScoreIndex(score);

        if (i < 0) return; // Score did not place in leaderboard.

        // Shift all proceeding scores down a slot.
        for (int j = Highscores.Length - 1; j != i; j--)
            Highscores[j] = Highscores[j - 1];

        // Insert the new score.
        Highscores[i] = new ScoreRecord(name, score);

        // Persist highscore updates.
        saveHighscores();

        EventManager.Instance.SetHighScores(Highscores);
    }

    public ScoreRecord getHighScore(int index)
    {
        return Highscores[index] ?? null;
    }

    [Serializable]
    private class ScoreRecordWrapper
    {
        public ScoreRecord[] highscores;
    }
}

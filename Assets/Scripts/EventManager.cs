using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    public event Action<int> OnScoreChanged;
    public event Action<int> OnGameOver;
    public event Action<GameState> OnSetGameState;
    public event Action<GameState, GameState> OnGameStateChanged;
    public event Action<ScoreRecord[]> OnHighScoresChanged;
    public event Action<int, string, int?> OnHighScoreSubmitted;
    public event Action<int, int> OnScoreRanked;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeGameState(GameState newState, GameState oldState)
    {
        OnGameStateChanged?.Invoke(newState, oldState);
    }

    public void ChangeScore(int value)
    {
        OnScoreChanged?.Invoke(value);
    }

    public void GameOver(int finalScore)
    {
        OnGameOver?.Invoke(finalScore);
    }

    public void RankScore(int score, int rank)
    {
        OnScoreRanked?.Invoke(score, rank);
    }

    public void SetGameState(GameState newState)
    {
        OnSetGameState?.Invoke(newState);
    }

    public void SetHighScores(ScoreRecord[] highScores)
    {
        OnHighScoresChanged?.Invoke(highScores);
    }

    public void SetScore(int score)
    {
        OnScoreChanged?.Invoke(score);
    }

    public void SubmitHighScore(int score, string name, int? rank)
    {
        OnHighScoreSubmitted?.Invoke(score, name, rank);
    }
}

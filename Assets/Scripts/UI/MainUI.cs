using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _currentScoreText;
    [SerializeField] private TextMeshProUGUI _topScoreText;

    [Header("Screens")]
    [SerializeField] private GameObject _gameOverScreen;
    [SerializeField] private GameObject _newHighScoreScreen;

    [Header("High Score Form")]
    [SerializeField] private TextMeshProUGUI _rankHeader;
    [SerializeField] private TMP_InputField _nameInput;

    private string _playerName;
    private int _scoreRank;
    private int _score;

    private void Awake()
    {
        _gameOverScreen.SetActive(false);
        _newHighScoreScreen.SetActive(false);

        ScoreRecord topScore = ScoreManager.Instance.getHighScore(0);
        if (topScore == null)
            _topScoreText.SetText("");
        else
            setTopScoreText(topScore.name, topScore.score);

        EventManager.Instance.OnGameStateChanged += handleGameStateChanged;
        EventManager.Instance.OnScoreChanged += setScore;
        EventManager.Instance.OnScoreRanked += handleScoreRanked;
        EventManager.Instance.OnHighScoresChanged += handleHighScoresChanged;
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnGameStateChanged -= handleGameStateChanged;
        EventManager.Instance.OnScoreChanged -= setScore;
        EventManager.Instance.OnScoreRanked -= handleScoreRanked;
        EventManager.Instance.OnHighScoresChanged -= handleHighScoresChanged;
    }

    private void handleGameStateChanged(GameState newState, GameState oldState)
    {
        switch(newState)
        {
            case GameState.GameOver:
                _topScoreText.gameObject.SetActive(false);
                _newHighScoreScreen.SetActive(false);
                _gameOverScreen.SetActive(true);
                break;
        }
    }

    private void handleHighScoresChanged(ScoreRecord[] highScores)
    {
        if (highScores.Length == 0 || highScores[0] == null)
            _topScoreText.SetText("");
        else
            setTopScoreText(highScores[0].name, highScores[0].score);
    }

    private void handleScoreRanked(int score, int rank)
    {
        if(rank == 0)
        {
            EventManager.Instance.SetGameState(GameState.GameOver);
            return;
        }

        string rankText;

        switch(rank)
        {
            case 1:
                rankText = "1st";
                break;

            case 2:
                rankText = "2nd";
                break;

            case 3:
                rankText = "3rd";
                break;

            default:
                rankText = $"{rank}th";
                break;
        }

        _rankHeader.SetText($"You've Ranked {rankText}!");
        _scoreRank = rank;
        _score = score;
        _newHighScoreScreen.SetActive(true);
        _nameInput.ActivateInputField();
    }

    private void setScore(int value)
    {
        _currentScoreText.SetText($"Score : {value}");
    }

    private void setTopScoreText(string name, int score)
    {
        if (name == "" || score == 0)
            _topScoreText.SetText("");
        else
            _topScoreText.SetText($"Top Score: {name} - {score}");
    }

    public void SubmitHighScore()
    {
        _playerName = _nameInput.text.Trim();

        if(_playerName != "")
            EventManager.Instance.SubmitHighScore(_score, _playerName, _scoreRank);

        EventManager.Instance.SetGameState(GameState.GameOver);
    }
}

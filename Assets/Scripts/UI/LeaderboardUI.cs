using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardUI : MonoBehaviour
{
    public static LeaderboardUI Instance;

    [SerializeField] private GameObject _scoresContainer;
    [SerializeField] private GameObject _scoreTextPrefab;
    [SerializeField] private GameObject _noScoresMessage;

    private GameObject[] _scoreTexts;
    private float _scoreMaxHeight;
    private RectTransform _scoreBaseTransform;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        EventManager.Instance.OnGameStateChanged += handleGameStateChanged;
        EventManager.Instance.OnHighScoresChanged += handleHighScoresChanged;

        _scoreBaseTransform = _scoreTextPrefab.GetComponent<RectTransform>();
        _scoreMaxHeight = _scoreBaseTransform.sizeDelta.y;
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnGameStateChanged -= handleGameStateChanged;
        EventManager.Instance.OnHighScoresChanged -= handleHighScoresChanged;
    }

    private void handleGameStateChanged(GameState newState, GameState oldState)
    {
        switch(newState)
        {
            case GameState.MainMenu:
                gameObject.SetActive(true);
                break;

            case GameState.NewGame:
                gameObject.SetActive(false);
                break;

            case GameState.GameOver:
                gameObject.SetActive(true);
                break;
        }
    }

    private void handleHighScoresChanged(ScoreRecord[] highScores)
    {
        if (_scoreTexts == null || _scoreTexts.Length < highScores.Length)
            _scoreTexts = new GameObject[highScores.Length];

        for(int i = 0; i < highScores.Length; i++)
        {
            if (highScores[i] == null || highScores[i].name == null) return;

            if (_scoreTexts[i] == null)
            {
                _scoreTexts[i] = Instantiate(_scoreTextPrefab, _scoreTextPrefab.transform.position, _scoreTextPrefab.transform.rotation, _scoresContainer.transform);
                _scoreTexts[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(
                    _scoreBaseTransform.anchoredPosition.x,
                    _scoreBaseTransform.anchoredPosition.y - _scoreMaxHeight * i
                );
            }

            _scoreTexts[i].GetComponent<TextMeshProUGUI>().SetText($"#{i + 1} {highScores[i].name} - {highScores[i].score}");
        }

        if (highScores.Length == 0 || highScores[0] == null || highScores[0].name == "")
            _noScoresMessage.SetActive(true);
        else
            _noScoresMessage.SetActive(false);
    }
}

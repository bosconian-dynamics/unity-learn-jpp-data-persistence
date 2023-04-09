using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    [SerializeField] private Brick _brickPrefab;
    [SerializeField] private int _lineCount = 6;
    [SerializeField] private Rigidbody _ball;

    private bool _hasStarted = false;
    private int _points;
    private bool _isGameOver = false;

    private void Awake()
    {
        EventManager.Instance.OnGameStateChanged += handleGameStateChanged;
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnGameStateChanged -= handleGameStateChanged;
    }

    // Start is called before the first frame update
    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < _lineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(_brickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(addPoint);
            }
        }
    }

    private void Update()
    {
        if (!_hasStarted)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _hasStarted = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                _ball.transform.SetParent(null);
                _ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (_isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                EventManager.Instance.SetGameState(GameState.ExitGame);
            }
        }
    }

    private void addPoint(int point)
    {
        _points += point;

        EventManager.Instance.SetScore(_points);
    }

    private void handleGameStateChanged(GameState newState, GameState oldState)
    {
        switch(newState)
        {
            case GameState.EndGame:
                EventManager.Instance.GameOver(_points);
                break;

            case GameState.GameOver:
                _isGameOver = true;
                break;
        }
    }
}

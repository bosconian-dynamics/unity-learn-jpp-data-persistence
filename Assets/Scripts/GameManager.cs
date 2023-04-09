using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; } = GameState.Default;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        EventManager.Instance.OnSetGameState += handleNewGameState;
        EventManager.Instance.OnGameStateChanged += handleGameStateChanged;
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnSetGameState -= handleNewGameState;
        EventManager.Instance.OnGameStateChanged -= handleGameStateChanged;
    }

    private void Start()
    {
        EventManager.Instance.SetGameState(GameState.Init);
    }

    private void handleGameStateChanged(GameState newState, GameState oldState)
    {
        switch (newState)
        {
            case GameState.NewGame:
                SceneManager.LoadScene("main");
                break;

            case GameState.ExitGame:
                SceneManager.LoadScene("Menu");
                break;
        }
    }

    private void handleNewGameState(GameState newState)
    {
        GameState oldState = CurrentState;
        CurrentState = newState;

        if (oldState == newState) return;

        EventManager.Instance.ChangeGameState(newState, oldState);
    }
}

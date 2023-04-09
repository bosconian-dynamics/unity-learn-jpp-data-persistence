using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    private void Awake()
    {
        //EventManager.Instance.OnHighScoresChanged += handleHighScoresChanged;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            StartGame();
    }

    public void StartGame()
    {
        EventManager.Instance.SetGameState(GameState.NewGame);
    }
}

﻿using FrameLord;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviorSingleton<GameManager>
{
    [HideInInspector]
    public GameMode gameMode;
    [HideInInspector]
    public GameState gameState;

    public GameObject pauseMenu;
    public int tournamentMatches = 3;
    public int exhibitionMatches = 1;
    public int difficulty = 1;
    public int gamesToWin = 2;

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene("MainMenu");
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameState == GameState.Paused)
                Resume();
            else if(gameState == GameState.Game)
                Pause();
        }
    }

    public enum GameState
    {
        MainMenu,
        Game,
        Paused
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        gameState = GameState.Game;
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        gameState = GameState.Paused;
    }

    public enum GameMode
    {
        Exhibition,
        Tournament
    }

    public void PlayExhibition()
    {
        gameMode = GameMode.Exhibition;
    }

    public void PlayTournament()
    {
        gameMode = GameMode.Tournament;
        PlayRolandGarros();
    }

    public void PlayUSOpen()
    {
        gameState = GameState.Game;
        SceneManager.LoadScene("USOpen");
    }

    public void PlayRolandGarros()
    {
        gameState = GameState.Game;
        SceneManager.LoadScene("RolandGarros");
    }

    public void SetEasy()
    {
        difficulty = 1;
        PlayRolandGarros();
    }

    public void SetMedium()
    {
        difficulty = 2;
        PlayRolandGarros();
    }
    public void SetHard()
    {
        difficulty = 3;
        PlayRolandGarros();
    }

    public void ReturnToMenu()
    {
        pauseMenu.SetActive(false);
        gameState = GameState.MainMenu;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public GameMode GetGameMode()
    {
        return gameMode;
    }

    public int GetTournamentMatches()
    {
        return tournamentMatches;
    }

    public int GetExhibitionMatches()
    {
        return exhibitionMatches;
    }

    public int GetDifficulty()
    {
        return difficulty;
    }

    public int GetGamesToWin()
    {
        return gamesToWin;
    }
}

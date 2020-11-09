using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public GameMode gameMode;

    public GameState gameState = GameState.MainMenu;

    public GameObject pauseMenu;

    // Start is called before the first frame update
    void Start()
    {
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

    public void ReturnToMenu()
    {
        pauseMenu.SetActive(false);
        gameState = GameState.MainMenu;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public GameMode gameMode;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
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
        SceneManager.LoadScene("USOpen");
    }

    public void PlayRolandGarros()
    {
        SceneManager.LoadScene("RolandGarros");
    }

    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }
}

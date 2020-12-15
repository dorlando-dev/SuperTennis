using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetExhibition()
    {
        gameManager.SetExhibition();
    }

    public void SetTournament()
    {
        gameManager.SetTournament();
    }

    public void SetEasy()
    {
        gameManager.SetEasy();
    }

    public void SetMedium()
    {
        gameManager.SetMedium();
    }

    public void SetHard()
    {
        gameManager.SetHard();
    }

    public void PlayUSOpen()
    {
        gameManager.PlayUSOpen();
    }

    public void PlayRolandGarros()
    {
        gameManager.PlayRolandGarros();
    }

    public void SetSinglePLayer()
    {
        gameManager.SetSinglePLayer();
    }

    public void SetMultiPlayer()
    {
        gameManager.SetMultiPLayer();
    }

    public void SetJoystic()
    {
        gameManager.SetJoystick();
    }

    public void SetKeyboard()
    {
        gameManager.SetKeyboard();
    }

    public void QuitGame()
    {
        gameManager.QuitGame();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    public GameObject ball;
    public GameObject P1;
    public GameObject P2;

    private GameManager gameManager;
    private int gamesToWin;
    private int difficulty;
    private string scoreP1 = "0";
    private string scoreP2 = "0";
    private int gamesP1 = 0;
    private int gamesP2 = 0;
    private int pointWinner;
    private int gameWinner;
    private int matchWinner;
    private bool isGameOver = false;
    private int matchesRemaining;


    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        
        GameManager.GameMode gameMode = gameManager.GetGameMode();
        if (gameMode == GameManager.GameMode.Tournament)
            matchesRemaining = gameManager.GetTournamentMatches();
        else
            matchesRemaining = gameManager.GetExhibitionMatches();

        gamesToWin = gameManager.GetGamesToWin();
        difficulty = gameManager.GetDifficulty();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPointOver())
        {            
            AddScore(pointWinner);
            UpdateVisualScore();
            if (isGameOver)
            {
                if (IsMatchOver())
                {
                    //Display match winner text
                    if(matchesRemaining > 0 && matchWinner == 1)
                    {
                        ResetMatch();
                    }
                    else
                    {
                        if(matchWinner == 1)
                        {
                            //Display congratulations message
                        }
                        else
                        {
                            //Display lost message
                        }
                        gameManager.ReturnToMenu();
                    }
                }
                else
                {
                    //Display game winner text
                    ResetGame();
                }
            }
            else
                ResetPoint();
        }   
    }

    private bool IsPointOver()
    {
        //Como verga lo determinamos?
        return false;
    }

    private void AddScore(int player)
    {
        string currentScore = "0";
        string oponentScore = "0";
        if (player == 1)
        {
            currentScore = scoreP1;
            oponentScore = scoreP2;
        }
        else if (player == 2)
        {
            currentScore = scoreP2;
            oponentScore = scoreP1;
        }
        switch (currentScore)
        {
            case "0":
                currentScore = "15";
                break;
            case "15":
                currentScore = "30";
                break;
            case "30":
                currentScore = "40";
                break;
            case "40":
                if (oponentScore == "40")
                    currentScore = "Ad";
                else if (oponentScore == "Ad")
                    oponentScore = "40";
                else
                {
                    isGameOver = true;
                    gameWinner = player;                    
                }
                break;
            case "Ad":
                isGameOver = true;
                gameWinner = player;
                break;
        }

        if(player == 1)
        {
            scoreP1 = currentScore;
            scoreP2 = oponentScore;
            if (isGameOver)
                gamesP1++;
        }
        else if(player == 2)
        {
            scoreP1 = oponentScore;
            scoreP2 = currentScore;
            if (isGameOver)
                gamesP2++;
        }
    }

    private void UpdateVisualScore()
    {
        //Triggerear canvas component
    }

    private void ResetPoint()
    {
        //Reset de la posicion de los players y la pelota
    }

    private bool IsMatchOver()
    {
        if (gamesP1 >= gamesToWin)
        {
            matchWinner = 1;
            matchesRemaining--;
            return true;
        }
        if (gamesP2 >= gamesToWin)
        {
            matchWinner = 2;
            matchesRemaining--;
            return true;
        }
        return false;
    }

    private void ResetGame()
    {
        scoreP1 = "0";
        scoreP2 = "0";
        ResetPoint();
    }

    private void ResetMatch()
    {
        gamesP1 = 0;
        gamesP2 = 0;
        ResetGame();
    }
}

using FrameLord;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

public class MatchManager : MonoBehaviorSingleton<MatchManager>
{
    public GameObject ball;
    public GameObject P1;
    public GameObject P2;
    public List<Transform> P1Positions;
    public List<Transform> P2Positions;
    public TMP_Text textGameMode;
    public TMP_Text textMatch;
    public TMP_Text textScoreP1;
    public TMP_Text textScoreP2;
    public TMP_Text textGamesP1;
    public TMP_Text textGamesP2;

    private GameManager gameManager;
    private int gamesToWin;
    private int difficulty;
    private string scoreP1 = "0";
    private string scoreP2 = "0";
    private int gamesP1 = 0;
    private int gamesP2 = 0;
    private int pointWinner = 1;
    private int gameWinner;
    private int matchWinner;
    private bool isGameOver = false;
    private int matches = 0;
    private int matchesRemaining;
    private int servePosition = 0;
    private State state;
    private CourtPosition expectedServePosition;
    private int currentPlayer = 1;
    private bool ballBounced = false;
    private bool ballBouncedTwice = false;
    private CourtPosition bouncePosition;
    private bool firstServe = true;
    private int lastHit = 0;

    private enum State
    {
        Serve,
        Game,
        Out
    }

    public enum CourtPosition
    {
        NotSet,
        PlayerSquareRight,
        PlayerSquareLeft,
        PlayerHalf,
        OpponentSquareRight,
        OpponentSquareLeft,
        OpponentHalf,
        Out
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        
        GameManager.GameMode gameMode = gameManager.GetGameMode();
        if (gameMode == GameManager.GameMode.Tournament)
        {
            matchesRemaining = gameManager.GetTournamentMatches();
            textGameMode.text = "Tournament";
        }
        else
        {
            matchesRemaining = gameManager.GetExhibitionMatches();
            textGameMode.text = "Exhibition";
        }
        matches = matchesRemaining;
        textMatch.text = "1";
        gamesToWin = gameManager.GetGamesToWin();
        difficulty = gameManager.GetDifficulty();
        ResetPoint();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Serve:
                if (ballBounced)
                {
                    if (bouncePosition != CourtPosition.NotSet)
                    {
                        if (bouncePosition == expectedServePosition)
                            state = State.Game;
                        else
                        {
                            Debug.Log("Serve out!");
                            if (firstServe)
                            {
                                firstServe = false;
                                SetServePosition();
                                SetExpectedServePosition();
                                bouncePosition = CourtPosition.NotSet;
                                ballBounced = false;
                                ball.gameObject.GetComponent<Ball>().Freeze(true);
                                state = State.Serve;
                            }
                            else
                            {
                                firstServe = true;
                                state = State.Out;
                                pointWinner = currentPlayer == 1 ? 2 : 1;
                            }
                        }
                            
                    }
                }
                break;
            case State.Game:
                if (IsPointOver())
                {
                    state = State.Out;
                }
                break;
            case State.Out:
                AddScore(pointWinner);
                UpdateVisualScore();
                if (isGameOver)
                {
                    if (IsMatchOver())
                    {
                        //Display match winner text
                        if (matchesRemaining > 0 && matchWinner == 1)
                        {
                            ResetMatch();
                        }
                        else
                        {
                            if (matchWinner == 1)
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
                break;
        }          
    }

    private bool IsPointOver()
    {
        if (ballBounced)
        {
            if (bouncePosition == CourtPosition.Out)
            {
                if (lastHit == 1)
                    pointWinner = 2;
                else
                    pointWinner = 1;
                return true;
            }
            else if (lastHit == 1)
            {
                if (!ballBouncedTwice)
                {
                    if (bouncePosition == CourtPosition.PlayerHalf || bouncePosition == CourtPosition.PlayerSquareLeft || bouncePosition == CourtPosition.PlayerSquareRight)
                    {
                        pointWinner = 2;
                        return true;
                    }
                }
                else
                {
                    pointWinner = 2;
                    return true;
                }
            }
            else if (lastHit == 2)
            {
                if (!ballBouncedTwice)
                {
                    if (bouncePosition == CourtPosition.OpponentHalf || bouncePosition == CourtPosition.OpponentSquareLeft || bouncePosition == CourtPosition.OpponentSquareRight)
                    {
                        pointWinner = 1;
                        return true;
                    }
                }
                else
                {
                    pointWinner = 1;
                    return true;
                }
            }
        }
        return false;
    }

    public void SetLastHit(int player)
    {
        lastHit = player;
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
        textGamesP1.text = $"{gamesP1}";
        textGamesP2.text = $"{gamesP2}";
        textScoreP1.text = $"{scoreP1}";
        textScoreP2.text = $"{scoreP2}";
    }    

    private void ResetPoint()
    {
        SetServePosition();
        SetExpectedServePosition();
        bouncePosition = CourtPosition.NotSet;
        ballBounced = false;
        ballBouncedTwice = false;
        ball.gameObject.GetComponent<Ball>().Freeze(true);
        state = State.Serve;
        UpdateVisualScore();
    }

    private void SetServePosition()
    {
        SetPlayerPosition(new Vector3(P1Positions[servePosition].position.x, P1Positions[servePosition].position.y, P1Positions[servePosition].position.z));
        P2.transform.position = new Vector3(P2Positions[servePosition].position.x, P2Positions[servePosition].position.y, P2Positions[servePosition].position.z);
        if (!firstServe)
        {
            if (servePosition == 0)
                servePosition = 1;
            else
                servePosition = 0;
        }
        if (currentPlayer == 1)
            ball.transform.position = new Vector3(P1.transform.position.x, 3, P1.transform.position.z);
        else
            ball.transform.position = new Vector3(P2.transform.position.x, 3, P2.transform.position.z);
    }

    private void SetPlayerPosition(Vector3 position)
    {
        P1.gameObject.GetComponent<CharacterController>().enabled = false;
        P1.transform.position = position;
        P1.gameObject.GetComponent<CharacterController>().enabled = true;
    }

    private void ChangeCurrentPLayer()
    {
        if (currentPlayer == 1)
            currentPlayer = 2;
        else
            currentPlayer = 1;
    }

    private void SetExpectedServePosition()
    {
        if (currentPlayer == 1 && servePosition == 1)
            expectedServePosition = CourtPosition.OpponentSquareLeft;
        else if (currentPlayer == 1 && servePosition == 2)
            expectedServePosition = CourtPosition.OpponentSquareRight;
        else if (currentPlayer == 2 && servePosition == 1)
            expectedServePosition = CourtPosition.PlayerSquareRight;
        else
            expectedServePosition = CourtPosition.PlayerSquareLeft;
    }

    private bool IsMatchOver()
    {
        if (gamesP1 >= gamesToWin)
        {
            matchWinner = 1;
            matchesRemaining--;
            textMatch.text = $"{matches-matchesRemaining}";
            return true;
        }
        if (gamesP2 >= gamesToWin)
        {
            matchWinner = 2;
            matchesRemaining--;
            textMatch.text = $"{matches - matchesRemaining}";
            return true;
        }
        return false;
    }

    private void ResetGame()
    {
        scoreP1 = "0";
        scoreP2 = "0";
        ChangeCurrentPLayer();
        ResetPoint();
    }

    private void ResetMatch()
    {
        gamesP1 = 0;
        gamesP2 = 0;
        ResetGame();
    }

    public void SetBouncePosition(CourtPosition courtPosition)
    {
        bouncePosition = courtPosition;
        if (ballBounced)
            ballBouncedTwice = true;
        else
            ballBounced = true;
    }
}

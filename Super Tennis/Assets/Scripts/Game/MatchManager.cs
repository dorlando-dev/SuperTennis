using FrameLord;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

public class MatchManager : MonoBehaviorSingleton<MatchManager>
{
    public GameObject ball;
    public GameObject P1;
    public GameObject OtherPlayer;
    public GameObject AI;
    public GameObject P2;
    public List<Transform> P1Positions;
    public List<Transform> P2Positions;
    public TMP_Text textGameMode;
    public TMP_Text textMatch;
    public TMP_Text textScoreP1;
    public TMP_Text textScoreP2;
    public TMP_Text textGamesP1;
    public TMP_Text textGamesP2;
    public TMP_Text textPointWinner;
    public GameObject goPointWinner;
    public AudioSource audioClipHitP1Wins;
    public AudioSource audioClipHitP2Wins;
    public Camera camera2;

    private GameManager gameManager;
    private bool multiplayer;
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
    private int lastHit = 0;

    private float waitCounter = 0f;
    private float waitTime = 3f;

    private enum State
    {
        Serve,
        Game,
        WaitPointOver,
        Out,
        WaitGameOver,
        WaitMatchOver
    }

    public enum AIState
    {
        WaitingToServe,
        Serve,
        HitBall,
        WaitingForPlayerServe,
        MoveToCenter,
        MovingToBall,
        WaitAnimation,
        Stop
    }

    public enum PlayerState
    {
        Serve,
        Play,
        WaitAnimation
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
            multiplayer = gameManager.GetMultiplayer();
        }
        if (multiplayer)
        {
            P2 = OtherPlayer;
            Camera.main.rect = new Rect(0f, 0f, 0.495f, 1f);
            camera2.rect = new Rect(0.505f, 0f, 0.495f, 1f);
            camera2.gameObject.SetActive(true);
        }
        else
            P2 = AI;
        P2.SetActive(true);

        matches = matchesRemaining;
        matchesRemaining--;
        textMatch.text = "1";
        gamesToWin = gameManager.GetGamesToWin();
        difficulty = gameManager.GetDifficulty();
        if(!multiplayer)
            P2.GetComponent<AI>().SetDifficulty(difficulty);
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
                        {
                            state = State.Game;
                            Debug.Log("A");
                        }
                        else
                        {
                            Debug.Log("B");
                            state = State.WaitPointOver;
                            pointWinner = currentPlayer == 1 ? 2 : 1;
                            TriggerSound();
                            if (!multiplayer)
                            {
                                AI p2 = P2.GetComponent<AI>();
                                p2.SetState(AIState.Stop);
                            }

                        }
                    }
                }
                break;
            case State.Game:
                if (IsPointOver())
                {
                    TriggerSound();
                    state = State.WaitPointOver;
                    if(!multiplayer)
                        P2.GetComponent<AI>().SetState(AIState.Stop);
                }
                break;
            case State.WaitPointOver:
                DisplayPointResult("POINT " + (pointWinner == 1 ? "P1" : "P2"));
                if (waitCounter < waitTime)
                    waitCounter += Time.deltaTime;
                else
                {
                    goPointWinner.SetActive(false);
                    waitCounter = 0;
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
                        state = State.WaitMatchOver;
                    }
                    else
                    {
                        state = State.WaitGameOver;
                    }
                }
                else
                    ResetPoint();
                break;

            case State.WaitGameOver:
                DisplayPointResult("Game " + (gameWinner == 1 ? "P1" : "P2"));
                if (waitCounter < waitTime)
                    waitCounter += Time.deltaTime;
                else
                {
                    goPointWinner.SetActive(false);
                    waitCounter = 0;
                    ResetGame();
                }
                break;

            case State.WaitMatchOver:
                DisplayPointResult((matchWinner == 1 ? "P1" : "P2") + " wins");
                if (waitCounter < waitTime)
                    waitCounter += Time.deltaTime;
                else
                {
                    goPointWinner.SetActive(false);
                    waitCounter = 0;
                    if (matchesRemaining >= 0 && matchWinner == 1)
                    {
                        ResetMatch();
                    }
                    else
                    {
                        gameManager.ReturnToMenu();
                    }
                }
                break;
        }
    }

    private void TriggerSound()
    {
        if (pointWinner == 1)
            audioClipHitP1Wins.Play();
        else if (pointWinner == 2)
            audioClipHitP2Wins.Play();
    }

    private void DisplayPointResult(string text)
    {
        textPointWinner.text = text;
        goPointWinner.SetActive(true);
    }

    private bool IsPointOver()
    {
        if (ballBounced)
        {
            if(ballBouncedTwice)
            {
                if (lastHit == 1)
                    pointWinner = 1;
                else
                    pointWinner = 2;
                return true;
            }
            else if (bouncePosition == CourtPosition.Out)
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
        bouncePosition = CourtPosition.NotSet;
        return false;
    }

    public void SetLastHit(int player, Vector3 ballDestination)
    {
        lastHit = player;
        ballBounced = false;
        ballBouncedTwice = false;
        if(player == 1 && (state == State.Serve || state == State.Game))
        {
            if (!multiplayer)
            {
                P2.GetComponent<AI>().SetState(AIState.MovingToBall);
                P2.GetComponent<AI>().SetBallDestination(ballDestination);
            }
        }
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
        SetPlayerState();
        if (multiplayer)
            SetPlayer2State();
        else
            SetAIServeState();
        ChangeServePosition();
        bouncePosition = CourtPosition.NotSet;
        ballBounced = false;
        ballBouncedTwice = false;
        ball.gameObject.GetComponent<Ball>().Freeze(true);
        state = State.Serve;
        UpdateVisualScore();
    }

    private void SetPlayerState()
    {
        if(currentPlayer == 1)
            P1.GetComponent<KeyboardPlayer>().SetState(PlayerState.Serve);
        else
            P1.GetComponent<KeyboardPlayer>().SetState(PlayerState.Play);
    }

    private void SetPlayer2State()
    {
        if (currentPlayer == 1)
            P2.GetComponent<GamepadPlayer>().SetState(PlayerState.Play);
        else
            P2.GetComponent<GamepadPlayer>().SetState(PlayerState.Serve);
    }

    private void SetAIServeState()
    {
        if (currentPlayer == 1)
            P2.GetComponent<AI>().SetState(AIState.WaitingForPlayerServe);
        else
            P2.GetComponent<AI>().SetState(AIState.WaitingToServe);
    }

    private void ChangeServePosition()
    {
        if (servePosition == 0)
            servePosition = 1;
        else
            servePosition = 0;
    }

    private void SetServePosition()
    {
        SetPlayerPosition(new Vector3(P1Positions[servePosition].position.x, P1Positions[servePosition].position.y, P1Positions[servePosition].position.z));
        if(multiplayer)
            SetPlayer2Position(new Vector3(P2Positions[servePosition].position.x, P2Positions[servePosition].position.y, P2Positions[servePosition].position.z));
        else
            P2.transform.position = new Vector3(P2Positions[servePosition].position.x, P2Positions[servePosition].position.y, P2Positions[servePosition].position.z);
        P1.GetComponent<KeyboardPlayer>().SetServeSide(servePosition);
        if(!multiplayer)
        P2.GetComponent<AI>().SetServeSide(servePosition);
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

    private void SetPlayer2Position(Vector3 position)
    {
        P2.gameObject.GetComponent<CharacterController>().enabled = false;
        P2.transform.position = position;
        P2.gameObject.GetComponent<CharacterController>().enabled = true;
    }

    private void ChangeCurrentPlayer()
    {
        if (currentPlayer == 1)
            currentPlayer = 2;
        else
            currentPlayer = 1;
    }

    private void SetExpectedServePosition()
    {
        if (currentPlayer == 1 && servePosition == 0)
            expectedServePosition = CourtPosition.OpponentSquareLeft;
        else if (currentPlayer == 1 && servePosition == 1)
            expectedServePosition = CourtPosition.OpponentSquareRight;
        else if (currentPlayer == 2 && servePosition == 0)
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
            textMatch.text = $"{matches - matchesRemaining}";
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
        isGameOver = false;
        ChangeCurrentPlayer();
        servePosition = 0;
        ResetPoint();
    }

    private void ResetMatch()
    {
        gamesP1 = 0;
        gamesP2 = 0;
        difficulty++;
        if(!multiplayer)
            P2.GetComponent<AI>().SetDifficulty(difficulty);
        ResetGame();
    }

    public void SetBouncePosition(CourtPosition courtPosition)
    {
        if (ballBounced)
        {
            if (!ballBouncedTwice)
            {
                ballBouncedTwice = true;
                bouncePosition = courtPosition;
            }
        }
        else
        {
            ballBounced = true;
            bouncePosition = courtPosition;
        }
    }

    public int GetCurrentPlayer()
    {
        return currentPlayer;
    }
}

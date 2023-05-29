using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public enum GameState {
    MainMenu,
    Match,
    MatchEnd
}

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject ballPrefab;
    List<Ball> balls = new List<Ball>();
    [SerializeField] Transform ballsContainer;
    [SerializeField] GameObject MainPanel;
    [SerializeField] Paddle paddleLeft, paddleRight;

    [SerializeField] TextMeshProUGUI TMP_MiddleText;

    GameState gameState;

    int scoreToWin = 3;

    public static event Action OnGameStart;

    public int targetFrameRate = 60;

    private void Awake() {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;
        UI_ShowMainMenu();
    }
    private void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            if (gameState.Equals(GameState.MatchEnd)) {
                StartNewGame();
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (gameState.Equals(GameState.MatchEnd)) {
                UI_ShowMainMenu();
            }
        }
        if (Input.GetKeyDown(KeyCode.B)) {
            if (gameState.Equals(GameState.Match)) {
                SpawnBall(1f, Vector2.zero);
            }
        }
        if (gameState.Equals(GameState.Match)) {
            foreach (Ball ball in balls) {
                ball.UpdateVisualization();
            }
        }
    }
    private void FixedUpdate() {
        if (gameState.Equals(GameState.Match)) {
            if(paddleLeft.isAI)
                paddleLeft.UpdateHighPriorityTarget(balls);
            if(paddleRight.isAI)
                paddleRight.UpdateHighPriorityTarget(balls);
            paddleLeft.Move(Field.FieldSize.y / 2 + 1.5f);
            paddleRight.Move(Field.FieldSize.y / 2 + 1.5f);
            UpdateBalls();
        } 
    }
    public void UI_StartGame(int HumanPlayersCount) {
        if(HumanPlayersCount == 1) {
            paddleLeft.isAI = false;
            paddleRight.isAI = true;
        } else if (HumanPlayersCount == 2) {
            paddleLeft.isAI = false;
            paddleRight.isAI = false;
        } else {
            paddleLeft.isAI = true;
            paddleRight.isAI = true;
        }
        StartNewGame();
        UI_HideMainMenu();
    }
    public void UI_HideMainMenu() {
        MainPanel.SetActive(false);
    }
    public void UI_ShowMainMenu() {
        gameState = GameState.MainMenu;
        TMP_MiddleText.gameObject.SetActive(false);
        MainPanel.SetActive(true);
    }
    public void SpawnBall(float delayInSeconds, Vector2 pos) {
        Ball ballInstance = Instantiate(ballPrefab).GetComponent<Ball>();
        ballInstance.transform.SetParent(ballsContainer);
        balls.Add(ballInstance);
        ballInstance.ResetBall();
        StartCoroutine(BeginRoundAfterDelay(ballInstance, delayInSeconds));
    }
    void ClearAllBalls() {
        balls.Clear();
        int childCount = ballsContainer.childCount;
        for (int i = childCount - 1; i >= 0; i--) {
            GameObject child = ballsContainer.GetChild(i).gameObject;
            Destroy(child);
        }
    }
    void StartNewGame() {
        gameState = GameState.Match;
        ClearAllBalls();
        TMP_MiddleText.gameObject.SetActive(false);
        OnGameStart?.Invoke();
        SpawnBall(1f, Vector2.zero);
    }
    private void UpdateBalls() {
        if (balls.Count > 0) {
            for (int i = balls.Count-1; i >= 0; i--) {
                balls[i].Move();
                if (paddleLeft.BounceXIfNeeded(balls[i])) {
                    //LaunchBall(balls[i].Position, balls[i].Velocity);
                    return;
                }
                if (paddleRight.BounceXIfNeeded(balls[i])) {
                    return;
                }
                if (CheckForGoal(balls[i].Position.x, balls[i])) {
                    return;
                }
            }
        }
    }
    void EndGame() {
        TMP_MiddleText.gameObject.SetActive(true);
        if(paddleLeft.Score > paddleRight.Score) {
            // Left wins
            TMP_MiddleText.SetText("Left Wins!");
        } else if (paddleLeft.Score < paddleRight.Score) {
            // Right wins
            TMP_MiddleText.SetText("Right Wins!");
        } else {
            // Tie (?)
            TMP_MiddleText.SetText("Tie");
        }
        ClearAllBalls();
        gameState = GameState.MatchEnd;
    }
    bool CheckForGoal(float x, Ball ball) {
        float xExtents = Field.FieldSize.x / 2 - ball.Extents;
        if (x < -xExtents) {
            // Goal for Paddle R
            if(paddleRight.ScorePoint(scoreToWin)) {
                EndGame();
            } else {
                ClearAllBalls();
                SpawnBall(1f, Vector2.zero);
            }
            return true;
        }
        else if (x > xExtents) {
            // Goal for Paddle L
            if (paddleLeft.ScorePoint(scoreToWin)) {
                EndGame();
            } else {
                ClearAllBalls();
                SpawnBall(1f, Vector2.zero);
            }
            return true;
        }
        return false;
    }
    IEnumerator BeginRoundAfterDelay(Ball ball, float delayInSeconds) {
        yield return new WaitForSeconds(delayInSeconds);
        if(ball != null) {
            ball.StartNewGame();
        }
        yield return null;
    }
}

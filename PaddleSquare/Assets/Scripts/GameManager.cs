using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject ballPrefab;
    List<Ball> balls = new List<Ball>();
    [SerializeField] Transform ballsContainer;


    [SerializeField] Paddle paddleLeft, paddleRight;

    [SerializeField] TextMeshPro TMP_MiddleText;

    bool isGameStarted = false;

    int scoreToWin = 3;

    public static event Action OnGameStart;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            if (!isGameStarted) {
                StartNewGame();
            }
        }
        if (Input.GetKeyDown(KeyCode.B)) {
            if (isGameStarted) {
                SpawnBall(1f, Vector2.zero);
            }
        }
        if (isGameStarted) {
            foreach (Ball ball in balls) {
                ball.UpdateVisualization();
            }
        }
    }

    public void SpawnBall(float delayInSeconds, Vector2 pos) {
        Ball ballInstance = Instantiate(ballPrefab).GetComponent<Ball>();
        ballInstance.transform.SetParent(ballsContainer);
        balls.Add(ballInstance);
        ballInstance.ResetBall();
        StartCoroutine(BeginRoundAfterDelay(ballInstance, delayInSeconds));
    }

    public void LaunchBall(Vector2 pos, Vector2 velocity) {
        Ball ballInstance = Instantiate(ballPrefab).GetComponent<Ball>();
        ballInstance.transform.SetParent(ballsContainer);
        balls.Add(ballInstance);
        float bias = UnityEngine.Random.Range(-2f, 2f);
        ballInstance.LaunchBall(pos+Vector2.right*2, new Vector2(velocity.x, velocity.y + bias));
    }

    void ClearAllBalls() {
        balls.Clear();
        int childCount = ballsContainer.childCount;
        for (int i = childCount - 1; i >= 0; i--) {
            GameObject child = ballsContainer.GetChild(i).gameObject;
            Destroy(child);
        }
    }

    private void Awake() {
        TMP_MiddleText.gameObject.SetActive(false);
    }
    void StartNewGame() {
        ClearAllBalls();
        TMP_MiddleText.gameObject.SetActive(false);
        isGameStarted = true;
        OnGameStart?.Invoke();
        SpawnBall(1f, Vector2.zero);
    }

    private void FixedUpdate() {
        if (isGameStarted) {
            paddleLeft.UpdateHighPriorityTarget(balls);
            paddleRight.UpdateHighPriorityTarget(balls);
            paddleLeft.Move(Field.FieldSize.y / 2 + 1.5f);
            paddleRight.Move(Field.FieldSize.y / 2 + 1.5f);
            UpdateBalls();

        } 
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
        isGameStarted = false;
    }

    bool CheckForGoal(float x, Ball ball) {
        float xExtents = Field.FieldSize.x / 2 - ball.Extents;
        if (x < -xExtents) {
            // Goal for Paddle R
            if(paddleRight.ScorePoint(scoreToWin)) {
                EndGame();
            }
            ClearAllBalls();
            SpawnBall(1f, Vector2.zero);
            return true;
        }
        else if (x > xExtents) {
            // Goal for Paddle L
            if (paddleLeft.ScorePoint(scoreToWin)) {
                EndGame();
            }
            ClearAllBalls();
            SpawnBall(1f, Vector2.zero);
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

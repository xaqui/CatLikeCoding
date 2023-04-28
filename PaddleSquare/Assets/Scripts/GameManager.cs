using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Ball ball;

    [SerializeField] Paddle paddleLeft, paddleRight;

    [SerializeField] TextMeshPro TMP_ScoreLeft;
    [SerializeField] TextMeshPro TMP_ScoreRight;
    [SerializeField] TextMeshPro TMP_MiddleText;

    bool isGameStarted = false;
    int scoreLeft = 0;
    int scoreRight = 0;
    int scoreToWin = 3;

    public static event Action OnGameStart;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            if (!isGameStarted) {
                StartNewGame();
            }
        }
        if (isGameStarted) {
            ball.UpdateVisualization();
        }
    }
    void StartNewGame() {
        TMP_MiddleText.gameObject.SetActive(false);
        isGameStarted = true;
        OnGameStart?.Invoke();
        scoreLeft = 0;
        scoreRight = 0;
        TMP_ScoreLeft.SetText("0");
        TMP_ScoreRight.SetText("0");
    }
    private void FixedUpdate() {
        if (isGameStarted) {
            paddleLeft.Move(ball.Position.y, Field.FieldSize.y / 2 + 1.5f);
            paddleRight.Move(ball.Position.y, Field.FieldSize.y / 2 + 1.5f);
            ball.Move();
            BounceYIfNeeded(ball.Position.y);
            BounceXIfNeeded(ball.Position);
            CheckForGoal(ball.Position.x);
        } 
    }

    void EndGame() {
        TMP_MiddleText.gameObject.SetActive(true);
        if(scoreLeft > scoreRight) {
            // Left wins
            TMP_MiddleText.SetText("Left Wins!");
        } else if (scoreLeft < scoreRight) {
            // Right wins
            TMP_MiddleText.SetText("Right Wins!");
        } else {
            // Tie
            TMP_MiddleText.SetText("Tie");
        }
        ball.gameObject.SetActive(false);
        isGameStarted = false;
    }

    bool CheckForGoal(float x) {
        float xExtents = Field.FieldSize.x / 2 - ball.Extents;
        if (x < -xExtents) {
            // Goal for Paddle R
            ResetBallPosition();
            AddScore(false);
            if(scoreLeft >= scoreToWin || scoreRight >= scoreToWin) {
                EndGame();
            }
            return true;
        }
        else if (x > xExtents) {
            // Goal for Paddle L
            ResetBallPosition();
            AddScore(true);
            if (scoreLeft >= scoreToWin || scoreRight >= scoreToWin) {
                EndGame();
            }
            return true;
        }
        return false;
    }

    void ResetBallPosition() {
        ball.ResetBall();
        StartCoroutine(BeginRoundAfterDelay(1f));
    }

    IEnumerator BeginRoundAfterDelay(float delayInSeconds) {
        yield return new WaitForSeconds(delayInSeconds);
        ball.StartNewGame();
        yield return null;
    }
   void AddScore(bool isLeft) {
        if(isLeft) {
            scoreLeft++;
            TMP_ScoreLeft.text = scoreLeft + "";
        } else {
            scoreRight++;
            TMP_ScoreRight.text = scoreRight + "";
        }
    }
    void BounceYIfNeeded(float y) {
        float yExtents = Field.FieldSize.y/2 - ball.Extents;
        if (y < -yExtents) {
            ball.BounceY(-yExtents);
        }
        else if (y > yExtents) {
            ball.BounceY(yExtents);
        }
    }
    void BounceXIfNeeded(Vector2 ballPosition) {
        Vector2 HitPoint;
        float HitFactor;
        if (paddleLeft.HitBall(ballPosition, ball.Extents,out HitPoint, out HitFactor)) {
            BounceX(paddleLeft, HitPoint, HitFactor);
        } else if(paddleRight.HitBall(ballPosition, ball.Extents, out HitPoint, out HitFactor)) {
            BounceX(paddleRight, HitPoint, HitFactor);
        }
    }
    void BounceX(Paddle defender, Vector2 HitPoint, float HitFactor) {
        ball.BounceX(HitPoint.x);
        ball.IncreaseSpeed(HitFactor *1.1f);
        defender.InitiateCooldown();
    }
}

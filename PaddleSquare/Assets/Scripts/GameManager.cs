using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Ball ball;

    [SerializeField] Paddle paddleLeft, paddleRight;

    bool IsGameStarted = false;

    public static event Action OnGameStart;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            if (!IsGameStarted) {
                IsGameStarted = true;
                OnGameStart?.Invoke();
            }
        }
        if (IsGameStarted) {
            ball.UpdateVisualization();
        }
    }

    private void FixedUpdate() {
        if (IsGameStarted) {
            paddleLeft.Move(ball.Position.y, Field.FieldSize.y / 2 + 1.5f);
            paddleRight.Move(ball.Position.y, Field.FieldSize.y / 2 + 1.5f);
            ball.Move();
            BounceYIfNeeded(ball.Position.y);
            BounceXIfNeeded(ball.Position);
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
        if (paddleLeft.HitBall(ballPosition, ball.Extents,out HitPoint)) {
            BounceX(paddleLeft, HitPoint);
        } else if(paddleRight.HitBall(ballPosition, ball.Extents, out HitPoint)) {
            BounceX(paddleRight, HitPoint);
        }
    }

    void BounceX(Paddle defender, Vector2 HitPoint) {
        ball.BounceX(HitPoint.x);
        defender.InitiateCooldown();
    }
}

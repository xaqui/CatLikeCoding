using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField, Min(0f)]
    float
    maxYSpeed = 20f,
    maxStartYSpeed = 6f,
    constantXSpeed = 10f,
    extents = 0.5f;

    Vector2 position, velocity;
    public float Extents => extents;
    public Vector2 Position => position;
    public Vector2 Velocity => velocity;

    private void Awake() {
        GameManager.OnGameStart += StartNewGame;
        gameObject.SetActive(false);
    }

    public void UpdateVisualization() =>
        transform.localPosition = new Vector3(position.x, 0f, position.y);

    public void Move() => position += velocity * Time.deltaTime;

    public void StartNewGame() {
        gameObject.SetActive(true);
        position = Vector2.zero;
        UpdateVisualization();
        velocity.y = Random.Range(-maxStartYSpeed, maxStartYSpeed);
        velocity.x = -constantXSpeed;
    }
    public void ResetBall() {
        position = Vector2.zero;
        velocity = Vector2.zero;
        UpdateVisualization();
    }
    public void IncreaseSpeed(float speedFactor) {
        velocity.y = maxYSpeed * speedFactor;
    }
    public void SetYPositionAndSpeed(float start, float speedFactor, float deltaTime) {
        velocity.y = maxYSpeed * speedFactor;
        position.y = start + velocity.y * deltaTime;
    }
    public void BounceX(float boundary) {
        //position.x = 2f * boundary - position.x;
        velocity.x = -velocity.x;
    }
    public void BounceY(float boundary) {
        position.y = 2f * boundary - position.y;
        velocity.y = -velocity.y;
    }

}

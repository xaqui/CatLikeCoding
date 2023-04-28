using System.Collections;
using UnityEngine;

public abstract class Paddle : MonoBehaviour
{
    [SerializeField] protected Transform Wall;

    [SerializeField] bool isAI;

    [SerializeField, Min(0f)] protected float speed = 10f;

    bool isActive = true;

    Vector2 extents => new Vector2(transform.localScale.x, transform.localScale.z);
    private void Awake() {
        GameManager.OnGameStart += Setup;
        gameObject.SetActive(false);
    }
    abstract protected void AlignToWall();
    abstract protected float AdjustByPlayer(float y);
    protected void Setup() {
        AlignToWall();
        gameObject.SetActive(true);
    }

    public void Move(float target, float arenaExtents) {
        Vector3 p = transform.localPosition;
        p.z = isAI ? AdjustByAI(p.z, target) : AdjustByPlayer(p.z);
        float limit = arenaExtents - extents.y;
        p.z = Mathf.Clamp(p.z, -limit, limit);
        transform.localPosition = p;
    }

    float AdjustByAI(float y, float target) {
        if (y < target) {
            return Mathf.Min(y + speed * Time.deltaTime, target);
        }
        return Mathf.Max(y - speed * Time.deltaTime, target);
    }
    public void InitiateCooldown() {
        if (isActive) {
            StartCoroutine(Cooldown(.5f));
        }
    }
    IEnumerator Cooldown(float timeInSeconds) {
        isActive = false;
        yield return new WaitForSeconds(timeInSeconds);
        isActive = true;
        yield return null;
    }

    public bool HitBall(Vector2 ballPosition, float ballExtents, out Vector2 HitPoint, out float HitFactor) {
        Vector2 paddlePosition2D = new Vector2(transform.localPosition.x, transform.localPosition.z);
        HitFactor =
            (ballPosition.y - paddlePosition2D.y) /
            (paddlePosition2D.y +extents.y/2 + ballExtents);

        Vector2 rectMin = paddlePosition2D - extents/2;
        Vector2 rectMax = paddlePosition2D + extents/2;

        float closestX = Mathf.Clamp(ballPosition.x, rectMin.x, rectMax.x);
        float closestY = Mathf.Clamp(ballPosition.y, rectMin.y, rectMax.y);
        HitPoint = new Vector2(closestX, closestY);
        if (!isActive) { return false; }

        float distance = Vector2.Distance(ballPosition, HitPoint);

        return distance < ballExtents;
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, new Vector3(extents.x, 1, extents.y));
    }

}

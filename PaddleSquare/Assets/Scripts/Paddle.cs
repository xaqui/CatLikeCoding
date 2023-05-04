using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public abstract class Paddle : MonoBehaviour
{
    [SerializeField] protected Transform Wall;
    [SerializeField] protected MeshRenderer GoalRenderer;
    [SerializeField] TextMeshPro TMP_Score;

    [SerializeField, ColorUsage(true, true)]
    Color goalColor = Color.white;

    [SerializeField] bool isAI;

    [SerializeField, Min(0f)] protected float speed = 10f,
        maxTargetingBias = 0.75f;

    float targetingBias;

    Material goalMaterial, paddleMaterial, scoreMaterial;

    bool isActive = true;
    public int Score { get; private set; } = 0;

    static readonly int
        emissionColorId = Shader.PropertyToID("_EmissionColor"),
        faceColorId = Shader.PropertyToID("_FaceColor"),
        timeOfLastHitId = Shader.PropertyToID("_TimeOfLastHit");

    Vector2 extents => new Vector2(transform.localScale.x, transform.localScale.z);

    void ChangeTargetingBias() =>
        targetingBias = Random.Range(-maxTargetingBias, maxTargetingBias);
    private void Awake() {
        TMP_Score.gameObject.SetActive(false);
        GameManager.OnGameStart += Setup;
        gameObject.SetActive(false);
        paddleMaterial = GetComponent<MeshRenderer>().material;
        goalMaterial = GoalRenderer.material;
        goalMaterial.SetColor(emissionColorId, goalColor);
        scoreMaterial = TMP_Score.fontMaterial;
        
    }
    abstract protected void AlignToWall();
    abstract protected float AdjustByPlayer(float y);
    protected void Setup() {
        AlignToWall();
        ChangeTargetingBias();
        gameObject.SetActive(true);
        SetScore(0);
        TMP_Score.gameObject.SetActive(true);
    }

    protected void SetScore(int scoreValue) {
        Score = scoreValue;
        TMP_Score.SetText(Score+"");
        scoreMaterial.SetColor(faceColorId, Color.white *1.5f);
    }

    public void Move(float target, float arenaExtents) {
        Vector3 p = transform.localPosition;
        p.z = isAI ? AdjustByAI(p.z, target) : AdjustByPlayer(p.z);
        float limit = arenaExtents - extents.y;
        p.z = Mathf.Clamp(p.z, -limit, limit);
        transform.localPosition = p;
    }

    float AdjustByAI(float y, float target) {
        target += targetingBias * extents.y;
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
            ((ballPosition - paddlePosition2D) /
            (extents + new Vector2(ballExtents, ballExtents))).y;

        Vector2 rectMin = paddlePosition2D - extents/2;
        Vector2 rectMax = paddlePosition2D + extents/2;

        float closestX = Mathf.Clamp(ballPosition.x, rectMin.x, rectMax.x);
        float closestY = Mathf.Clamp(ballPosition.y, rectMin.y, rectMax.y);
        HitPoint = new Vector2(closestX, closestY);
        if (!isActive) { return false; }

        float distance = Vector2.Distance(ballPosition, HitPoint);

        bool success = distance < ballExtents;

        if (success) {
            paddleMaterial.SetFloat(timeOfLastHitId, Time.time);
        }

        return success;
    }

    public bool ScorePoint(int pointsToWin) {
        goalMaterial.SetFloat(timeOfLastHitId, Time.time);
        SetScore(Score + 1);
        return Score >= pointsToWin;
    }

    

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, new Vector3(extents.x, 1, extents.y));
    }

}

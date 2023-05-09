using UnityEngine;
using static UnityEngine.ParticleSystem;

public struct SerializedBallData {
    public SerializedBallData(float VelocityX, float Distance, Ball ballInstance) {
        this.VelocityX = VelocityX;
        this.Distance = Distance;
        this.ballInstance = ballInstance;
    }
    public Ball ballInstance { get; }
    public float VelocityX { get; }
    public float Distance { get; }
}

public class Ball : MonoBehaviour
{
    [SerializeField, Min(0f)]
    float
    maxYSpeed = 20f,
    maxStartYSpeed = 6f,
    constantXSpeed = 10f,
    extents = 0.5f;

    [SerializeField]
    ParticleSystem bounceParticleSystem, trailParticleSystem;

    [SerializeField]
    int bounceParticleEmission = 20;

    [SerializeField]
    Color[] BounceColorsPrimary;
    [SerializeField]
    Color[] BounceColorsSecondary;

    [SerializeField]
    Light PointLight;

    [SerializeField]
    AudioSource HitSound;

    Material ballMaterial;

    Vector2 position, velocity;
    public float Extents => extents;
    public Vector2 Position => position;
    public Vector2 Velocity => velocity;

    public bool isLive = false;
    public bool canCollide = true;
    public int stepsSinceLastBounce;

    public void UpdateVisualization() =>
        transform.localPosition = new Vector3(position.x, 0f, position.y);
    public void Move() => position += velocity * Time.deltaTime;

    private void Awake() {
        ballMaterial = GetComponent<MeshRenderer>().material;
    }

    private void FixedUpdate() {
        stepsSinceLastBounce++;
        if (isLive) {
            BounceYIfNeeded(position.y);
        }
    }

    public void StartNewGame() {
        isLive = true;
        UpdateVisualization();
        velocity.y = Random.Range(-maxStartYSpeed, maxStartYSpeed);

        velocity.x = (Random.Range(0, 2) * 2 - 1) * constantXSpeed;
    }
    public void ResetBall() {
        SetTrailEmission(false);
        position = Vector2.zero;
        velocity = Vector2.zero;
        UpdateVisualization();
        SetTrailEmission(true);
        trailParticleSystem.Play();
        RandomizeParticlesColor();
    }
    public void LaunchBall(Vector2 pos, Vector2 vel) {
        SetTrailEmission(false);
        position = pos;
        velocity = vel;
        UpdateVisualization();
        SetTrailEmission(true);
        trailParticleSystem.Play();
        RandomizeParticlesColor();
        isLive = true;
    }
    public void IncreaseSpeed(float speedFactor) {
        float velocityY = maxYSpeed * speedFactor;
        if(velocityY == 0) {
            velocityY = maxStartYSpeed;
        }
        velocity.x += .3f;
        velocity.y = velocityY;
    }
    public void CalculateBounceTime() {
        if(stepsSinceLastBounce < 10) {
            canCollide = false;
        } else {
            canCollide = true;
        }
    }
    public void BounceX(float boundary) {
        CalculateBounceTime();
        if (canCollide) {
            stepsSinceLastBounce = 0;
            velocity.x = -velocity.x;
            EmitBounceParticles(
                position.x,
                position.y,
                boundary < 0f ? 90f : 270f
            );
            HitSound.Play();
            RandomizeParticlesColor();
        }
    }

    public void BounceY(float boundary) {
        stepsSinceLastBounce = 0;
        position.y = 2f * boundary - position.y;
        velocity.y = -velocity.y;
        EmitBounceParticles(
            position.x,
            position.y,
            boundary < 0f ? 0f : 180f
        );
    }
    void BounceYIfNeeded(float y) {
        float yExtents = Field.FieldSize.y / 2 - Extents;
        if (y < -yExtents) {
            BounceY(-yExtents);
        }
        else if (y > yExtents) {
            BounceY(yExtents);
        }
    }
    void EmitBounceParticles(float x, float z, float rotation) {
        ShapeModule shape = bounceParticleSystem.shape;
        //shape.position = new Vector3(x, 0f, z);
        shape.rotation = new Vector3(0f, rotation, 0f);
        bounceParticleSystem.Emit(bounceParticleEmission);
    }
    void SetTrailEmission(bool enabled) {
        EmissionModule emission = trailParticleSystem.emission;
        emission.enabled = enabled;
    }
    void RandomizeParticlesColor() {
        int indexRandom = Random.Range(0, BounceColorsPrimary.Length);
        Color col1 = BounceColorsPrimary[indexRandom];
        Color col2 = BounceColorsSecondary[indexRandom];

        ColorOverLifetimeModule col = trailParticleSystem.colorOverLifetime;

        Gradient gradient = CreateGradient(col1, col2);

        col.color = gradient ;
        PointLight.color = col1;
        ballMaterial.SetColor("_EmissionColor", col1*10);
        col = bounceParticleSystem.colorOverLifetime;
        col.color = gradient;
    }
    Gradient CreateGradient(Color col1, Color col2) {
        Gradient gradient = new Gradient();

        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0].color = col1;
        colorKeys[0].time = 0f;
        colorKeys[1].color = col2;
        colorKeys[1].time = 1f;

        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[3];
        alphaKeys[0].alpha = 0f;
        alphaKeys[0].time = 0f;
        alphaKeys[1].alpha = 1f;
        alphaKeys[1].time = 0.1f;
        alphaKeys[2].alpha = 0f;
        alphaKeys[2].time = 1f;

        gradient.SetKeys(colorKeys, alphaKeys);

        return gradient;
    }
}

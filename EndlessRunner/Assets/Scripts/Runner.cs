using UnityEngine;

public class Runner : MonoBehaviour {
    [SerializeField]
    Light pointLight;

    [SerializeField]
    ParticleSystem explosionSystem, trailSystem;

    [SerializeField, Min(0f)]
    float startSpeedX = 5f;

    MeshRenderer meshRenderer;

    Vector2 position;

    public Vector2 Position => position;

    void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
        pointLight.enabled = false;
    }

    private void Start() {
        StartNewGame();
    }

    public void StartNewGame() {
        position = Vector2.zero;
        transform.localPosition = position;
        meshRenderer.enabled = true;
        pointLight.enabled = true;
        explosionSystem.Clear();
        SetTrailEmission(true);
        trailSystem.Clear();
        trailSystem.Play();
    }

    void Explode() {
        meshRenderer.enabled = false;
        pointLight.enabled = false;
        SetTrailEmission(false);
        transform.localPosition = position;
        explosionSystem.Emit(explosionSystem.main.maxParticles);
    }

    void SetTrailEmission(bool enabled) {
        ParticleSystem.EmissionModule emission = trailSystem.emission;
        emission.enabled = enabled;
    }

    public bool Run(float dt) {
        if (position.x > 25f) {
            Explode();
            return false;
        }
        position.x += startSpeedX * dt;
        return true;
    }
    public void UpdateVisualization() {
        transform.localPosition = position;
    }
}
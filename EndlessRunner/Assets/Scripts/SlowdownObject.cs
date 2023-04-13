using UnityEngine;

public class SlowdownObject : SkylineObject {
    [SerializeField]
    Transform item;

    [SerializeField]
    ParticleSystem explosionSystem;

    [SerializeField]
    float radius = 1f;

    [SerializeField]
    float speedFactor = 0.75f;

    [SerializeField]
    float spawnProbability = 0.5f;

    public override void Check(Runner runner) {
        if (
        item.gameObject.activeSelf &&
        ((Vector2) item.position - runner.Position).sqrMagnitude < radius * radius
    ) {
            item.gameObject.SetActive(false);
            explosionSystem.Emit(explosionSystem.main.maxParticles);
            runner.SpeedX *= speedFactor;
        }
    }

    void OnEnable() {
        item.gameObject.SetActive(Random.value < spawnProbability);
        explosionSystem.Clear();
    }
}
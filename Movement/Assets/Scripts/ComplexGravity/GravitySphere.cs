using UnityEngine;

public class GravitySphere : GravitySource {

    [SerializeField]
    float gravity = 9.81f;

    [SerializeField, Min(0f)]
    float outerRadius = 10f, outerFalloffRadius = 15f;

    float outerFalloffFactor;

    void OnDrawGizmos() {
        Vector3 p = transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(p, outerRadius);
        if (outerFalloffRadius > outerRadius) {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(p, outerFalloffRadius);
        }
    }
    public override Vector3 GetGravity(Vector3 position) {
        Vector3 vector = transform.position - position;
        float distance = vector.magnitude;
        if (distance > outerFalloffRadius) {
            return Vector3.zero;
        }
        float g = gravity / distance;
        if (distance > outerRadius) {
            g *= 1f - (distance - outerRadius) * outerFalloffFactor;
        }
        return g * vector; 
    }

    void Awake() {
        OnValidate();
    }
    void OnValidate() {
        outerFalloffRadius = Mathf.Max(outerFalloffRadius, outerRadius);
        outerFalloffFactor = 1f / (outerFalloffRadius - outerRadius);
    }

}
using UnityEngine;

public class AccelerationZone : MonoBehaviour {

    [SerializeField, Min(0f)]
    float speed = 10f;

    void OnTriggerEnter(Collider other) {
        Rigidbody body = other.attachedRigidbody;
        if (body) {
            Accelerate(body);
        }
    }
    void Accelerate(Rigidbody body) {
        Vector3 velocity = body.velocity;
        if (velocity.y >= speed) {
            return;
        }

        velocity.y = speed;
        body.velocity = velocity;
    }
}
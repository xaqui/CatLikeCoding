using UnityEngine;

public class PositionInterpolator : MonoBehaviour {

    [SerializeField]
    Rigidbody body = default;

    [SerializeField]
    Vector3 from = default, to = default;

    public void Interpolate(float t) {
        body.MovePosition(Vector3.LerpUnclamped(from, to, t));
    }
}
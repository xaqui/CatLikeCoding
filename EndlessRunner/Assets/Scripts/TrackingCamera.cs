using UnityEngine;

public class TrackingCamera : MonoBehaviour {
    Vector3 offset, position;
    float viewFactorX;
    public FloatRange VisibleX(float z) =>
    FloatRange.PositionExtents(position.x, viewFactorX * (z - position.z));

    ParticleSystem stars;

    void Awake() {
        offset = transform.localPosition;

        Camera c = GetComponent<Camera>();
        float viewFactorY = Mathf.Tan(c.fieldOfView * 0.5f * Mathf.Deg2Rad);
        viewFactorX = viewFactorY * c.aspect;

        stars = GetComponent<ParticleSystem>();
        ParticleSystem.ShapeModule shape = stars.shape;
        Vector3 position = shape.position;
        position.y = viewFactorY * position.z * 0.5f;
        shape.position = position;
        shape.scale = new Vector3(2f * viewFactorX, viewFactorY) * position.z;
    }

    public void StartNewGame() {
        Track(Vector3.zero);
        stars.Clear();
        stars.Emit(stars.main.maxParticles);
    }

    public void Track(Vector3 focusPoint) {
        position = focusPoint + offset;
        transform.localPosition = position;
    }
}

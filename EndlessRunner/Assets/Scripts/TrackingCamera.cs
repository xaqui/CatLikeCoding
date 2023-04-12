using UnityEngine;

public class TrackingCamera : MonoBehaviour {
    Vector3 offset, position;

    void Awake() {
        offset = transform.localPosition;
    }

    public void StartNewGame() {
        Track(Vector3.zero);
    }

    public void Track(Vector3 focusPoint) {
        position = focusPoint + offset;
        transform.localPosition = position;
    }
}

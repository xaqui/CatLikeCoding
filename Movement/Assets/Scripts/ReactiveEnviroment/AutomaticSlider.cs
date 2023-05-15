using UnityEngine;
using UnityEngine.Events;

public class AutomaticSlider : MonoBehaviour {

    [SerializeField, Min(0.01f)]
    float duration = 1f;

    [System.Serializable]
    public class OnValueChangedEvent : UnityEvent<float> { }

    [SerializeField]
    OnValueChangedEvent onValueChanged = default;

    float value;

    void FixedUpdate() {
        value += Time.deltaTime / duration;
        if (value >= 1f) {
            value = 1f;
            enabled = false;
        }
        onValueChanged.Invoke(value);
    }
}
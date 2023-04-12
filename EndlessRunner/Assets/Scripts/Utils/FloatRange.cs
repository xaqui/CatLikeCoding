using UnityEngine;

[System.Serializable]
public struct FloatRange {
    public float min, max;

    public float RandomValue => Random.Range(min, max);

    public FloatRange(float min, float max) {
        this.min = min;
        this.max = max;
    }
    public FloatRange GrowExtents(float extents) =>
        new FloatRange(min - extents, max + extents);

    public FloatRange Shift(float shift) => new FloatRange(min + shift, max + shift);

    public static FloatRange PositionExtents(float position, float extents) =>
        new FloatRange(position - extents, position + extents);
}

using UnityEngine;

public class SkylineGenerator : MonoBehaviour {
    [SerializeField]
    SkylineObject[] prefabs;

    [SerializeField]
    float distance;

    [SerializeField]
    FloatRange altitude;

    const float border = 10f;

    Vector3 endPosition;

    SkylineObject leftmost, rightmost;

    SkylineObject GetInstance() {
        SkylineObject instance = prefabs[Random.Range(0, prefabs.Length)].GetInstance();
        instance.transform.SetParent(transform, false);
        return instance;
    }

    public void FillView(TrackingCamera view) {
        FloatRange visibleX = view.VisibleX(distance).GrowExtents(border);
        while (leftmost != rightmost && leftmost.MaxX < visibleX.min) {
            leftmost = leftmost.Recycle();
        }

        while (endPosition.x < visibleX.max) {
            endPosition.y = altitude.RandomValue;
            rightmost = rightmost.Next = GetInstance();
            endPosition = rightmost.PlaceAfter(endPosition);
        }
    }

    public void StartNewGame(TrackingCamera view) {
        while (leftmost != null) {
            leftmost = leftmost.Recycle();
        }

        FloatRange visibleX = view.VisibleX(distance).GrowExtents(border);
        endPosition = new Vector3(visibleX.min, altitude.RandomValue, distance);

        leftmost = rightmost = GetInstance();
        endPosition = rightmost.PlaceAfter(endPosition);
        FillView(view);
    }

}

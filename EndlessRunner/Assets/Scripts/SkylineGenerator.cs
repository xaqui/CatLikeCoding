using TMPro;
using UnityEngine;

public class SkylineGenerator : MonoBehaviour {
    [SerializeField]
    SkylineObject[] prefabs;

    [SerializeField]
    float distance;

    [SerializeField]
    FloatRange altitude;

    [SerializeField]
    SkylineObject gapPrefab;

    const float border = 10f;

    Vector3 endPosition;

    SkylineObject leftmost, rightmost;
    [SerializeField]
    FloatRange gapLength, sequenceLength;
    float sequenceEndX;

    SkylineObject GetInstance() {
        SkylineObject instance = prefabs[Random.Range(0, prefabs.Length)].GetInstance();
        instance.transform.SetParent(transform, false);
        return instance;
    }


    public SkylineObject StartNewGame(TrackingCamera view) {
        while (leftmost != null) {
            leftmost = leftmost.Recycle();
        }

        FloatRange visibleX = view.VisibleX(distance).GrowExtents(border);
        endPosition = new Vector3(visibleX.min, altitude.RandomValue, distance);
        sequenceEndX = sequenceLength.RandomValue;

        leftmost = rightmost = GetInstance();
        endPosition = rightmost.PlaceAfter(endPosition);
        FillView(view);
        return leftmost;
    }
    public void FillView(TrackingCamera view) {
        FloatRange visibleX = view.VisibleX(distance).GrowExtents(border);
        while (leftmost != rightmost && leftmost.MaxX < visibleX.min) {
            leftmost = leftmost.Recycle();
        }

        while (endPosition.x < visibleX.max) {
            if (endPosition.x > sequenceEndX) {
                StartNewSequence(gapLength.RandomValue, sequenceLength.RandomValue);
            }
            rightmost = rightmost.Next = GetInstance();
            endPosition = rightmost.PlaceAfter(endPosition);
        }
    }
    void StartNewSequence(float gap, float sequence) {
        if (gapPrefab != null) {
            rightmost = rightmost.Next = gapPrefab.GetInstance();
            rightmost.transform.SetParent(transform, false);
            rightmost.FillGap(endPosition, gap);
        }
        endPosition.x += gap;
        endPosition.y = altitude.RandomValue;
        sequenceEndX = endPosition.x + sequence;
    }

}

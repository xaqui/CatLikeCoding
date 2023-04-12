using UnityEngine;

public class SkylineObject : MonoBehaviour {

    [System.NonSerialized]
    System.Collections.Generic.Stack<SkylineObject> pool;
    public SkylineObject Next { get; set; }

    [SerializeField, Min(1f)]
    float extents;
    public float MaxX => transform.localPosition.x + extents;

    [SerializeField]
    FloatRange gapY;
    public FloatRange GapY => gapY.Shift(transform.localPosition.y);

    public SkylineObject GetInstance() {
        if (pool == null) {
            pool = new();
        }
        if (pool.TryPop(out SkylineObject instance)) {
            instance.gameObject.SetActive(true);
        }
        else {
            instance = Instantiate(this);
            instance.pool = pool;
        }
        return instance;
    }

    public SkylineObject Recycle() {
        pool.Push(this);
        gameObject.SetActive(false);
        SkylineObject n = Next;
        Next = null;
        return n;
    }

    public Vector3 PlaceAfter(Vector3 position) {
        position.x += extents;
        transform.localPosition = position;
        position.x += extents;
        return position;
    }
}
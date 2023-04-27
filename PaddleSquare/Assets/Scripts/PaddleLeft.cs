using UnityEngine;

public class PaddleLeft : Paddle {
    protected override void AlignToWall() {
        transform.SetLocalPositionAndRotation(new Vector3(Wall.transform.localPosition.x + Field.WALL_SEPARATION, 0, 0), Quaternion.identity);
    }
}

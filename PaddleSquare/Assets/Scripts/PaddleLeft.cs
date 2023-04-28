using UnityEngine;

public class PaddleLeft : Paddle {
    protected override void AlignToWall() {
        transform.SetLocalPositionAndRotation(new Vector3(Wall.transform.localPosition.x + Field.WALL_SEPARATION, 0, 0), Quaternion.identity);
    }
    override protected float AdjustByPlayer(float y) {
        bool goUp = Input.GetKey(KeyCode.W);
        bool goDown = Input.GetKey(KeyCode.S);
        if (goUp && !goDown) {
            return y + speed * Time.deltaTime;
        }
        else if (goDown && !goUp) {
            return y - speed * Time.deltaTime;
        }
        return y;
    }
}

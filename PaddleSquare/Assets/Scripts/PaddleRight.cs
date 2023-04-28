using UnityEngine;

public class PaddleRight : Paddle {
    protected override void AlignToWall() {
        transform.SetLocalPositionAndRotation(new Vector3(Wall.transform.localPosition.x - Field.WALL_SEPARATION, 0, 0), Quaternion.identity);
    }
    override protected float AdjustByPlayer(float y) {
        bool goUp = Input.GetKey(KeyCode.UpArrow);
        bool goDown = Input.GetKey(KeyCode.DownArrow);
        if (goUp && !goDown) {
            return y + speed * Time.deltaTime;
        }
        else if (goDown && !goUp) {
            return y - speed * Time.deltaTime;
        }
        return y;
    }
}

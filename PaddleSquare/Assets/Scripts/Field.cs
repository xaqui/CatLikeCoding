using UnityEngine;

public class Field : MonoBehaviour
{
    public readonly static float WALL_SEPARATION= 4.5f;
    
    [SerializeField] GameObject PosZWall;
    [SerializeField] GameObject NegZWall;
    [SerializeField] GameObject PosXWall;
    [SerializeField] GameObject NegXWall;
    [SerializeField] GameObject Net;

    public static readonly Vector2 FieldSize = new Vector2(40f,24f);

    private void Awake() {
        CalculateFieldBounds();
    }
    void CalculateFieldBounds() {
        NegXWall.transform.SetPositionAndRotation( new Vector3(-FieldSize.x / 2, 0, 0f), Quaternion.identity);
        PosXWall.transform.SetPositionAndRotation( new Vector3(FieldSize.x / 2, 0, 0f), Quaternion.identity);
        NegZWall.transform.SetPositionAndRotation( new Vector3(0, 0, -FieldSize.y / 2 + 0f), Quaternion.identity);
        PosZWall.transform.SetPositionAndRotation( new Vector3(0, 0, FieldSize.y / 2 + 0f), Quaternion.identity);
        NegZWall.transform.localScale = new Vector3(FieldSize.x-1,1,1);
        PosZWall.transform.localScale = new Vector3(FieldSize.x-1,1,1);
        NegXWall.transform.localScale = new Vector3(1,1,FieldSize.y+1);
        PosXWall.transform.localScale = new Vector3(1,1,FieldSize.y+1);
        Net.transform.localScale = new Vector3(.5f,FieldSize.y - 1, 1);
        Net.GetComponent<MeshRenderer>().material.mainTextureScale =new Vector2(1, FieldSize.y+10);
    }

}

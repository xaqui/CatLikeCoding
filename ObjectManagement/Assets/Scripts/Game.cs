using System.Collections.Generic;
using UnityEngine;

public class Game : PersistableObject
{
    public ShapeFactory shapeFactory;
    public KeyCode createKey = KeyCode.C;
    public KeyCode newGameKey = KeyCode.N;
    public KeyCode saveKey = KeyCode.S;
    public KeyCode loadKey = KeyCode.L;

    public PersistentStorage storage;
    List<Shape> shapes;

    void Awake() {
        shapes = new List<Shape>();
    }
    void Update() {
        if (Input.GetKeyDown(createKey)) {
            CreateObject();
        }
        else if (Input.GetKey(newGameKey)) {
            BeginNewGame();
        }
        else if (Input.GetKeyDown(saveKey)) {
            storage.Save(this);
        }
        else if (Input.GetKeyDown(loadKey)) {
            BeginNewGame();
            storage.Load(this);
        }
    }
    void CreateObject() {
        Shape instance = shapeFactory.GetRandom();
        Transform t = instance.transform;
        t.localPosition = Random.insideUnitSphere * 5f;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(0.1f, 1f);
        shapes.Add(instance);
    }
    void BeginNewGame() {
        for (int i = 0; i < shapes.Count; i++) {
            Destroy(shapes[i].gameObject);
        }
        shapes.Clear();
    }
    public override void Save(GameDataWriter writer) {
        writer.Write(shapes.Count);
        for (int i = 0; i < shapes.Count; i++) {
            shapes[i].Save(writer);
        }
    }
    public override void Load(GameDataReader reader) {
        int count = reader.ReadInt();
        for (int i = 0; i < count; i++) {
            Shape instance = shapeFactory.Get(0);
            instance.Load(reader);
            shapes.Add(instance);
        }
    }
}

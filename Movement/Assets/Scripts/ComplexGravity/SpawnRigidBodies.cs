using System.Collections.Generic;
using UnityEngine;

public class SpawnRigidBodies : MonoBehaviour
{
    const int MAX_SPAWN = 10;

    [SerializeField] GameObject PrefabToSpawn;
    Queue<GameObject> bodies = new Queue<GameObject>();
    private void Start() {
        SpawnRigidBody(transform.position, new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.O)) {
            SpawnRigidBody(transform.position, new Vector3(Random.Range(0,360), Random.Range(0, 360), Random.Range(0, 360)));
        }
    }

    void SpawnRigidBody(Vector3 position, Vector3 rotation) {
        if(bodies.Count >= MAX_SPAWN) {
            GameObject rbToDelete = bodies.Dequeue();
            Destroy(rbToDelete);
        }
        GameObject RigidBodyGameObject = Instantiate(PrefabToSpawn, position, Quaternion.identity);
        //RigidBodyGameObject.transform.localRotation = Quaternion.Euler(rotation);
        bodies.Enqueue(RigidBodyGameObject);
    }
}

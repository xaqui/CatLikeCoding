using TMPro;
using UnityEngine;

public class Game : MonoBehaviour {
    [SerializeField]
    Runner runner;

    [SerializeField]
    TrackingCamera trackingCamera;

    [SerializeField]
    TextMeshPro displayText;

    [SerializeField, Min(0.001f)]
    float maxDeltaTime = 1f / 120f;

    bool isPlaying;

    void StartNewGame() {
        trackingCamera.StartNewGame();
        runner.StartNewGame();
        isPlaying = true;
    }

    void Update() {
        if (isPlaying) {
            UpdateGame();
        }
        else if (Input.GetKeyDown(KeyCode.Space)) {
            StartNewGame();
        }
    }

    void UpdateGame() {
        float accumulateDeltaTime = Time.deltaTime;
        while (accumulateDeltaTime > maxDeltaTime && isPlaying) {
            isPlaying = runner.Run(maxDeltaTime);
            accumulateDeltaTime -= maxDeltaTime;
        }
        isPlaying = isPlaying && runner.Run(accumulateDeltaTime);
        isPlaying = runner.Run(Time.deltaTime);
        runner.UpdateVisualization();
        trackingCamera.Track(runner.Position);
        displayText.SetText("{0}", Mathf.Floor(runner.Position.x));
    }
}
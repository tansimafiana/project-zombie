using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadBackground : MonoBehaviour
{
    [SerializeField] private string[] sceneNames;
    [Space]

    [SerializeField] private float rotatingSpeed = 1f;
    private Transform rotatingCamTransform;
    private bool foundCam = false;

    void Start() {
        SceneManager.sceneLoaded += OnSceneLoaded;

        string selected = sceneNames[Random.Range(0, sceneNames.Length)];
        SceneManager.LoadScene(selected);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        SceneChanger.SignalSpectatorMode();
        Debug.Log("<color=yellow>Scene Loaded!\tName: " + scene.name + "\tMode: " + mode + "</color>");
        rotatingCamTransform = GameObject.Find("Panoramic Camera").transform;

        if (rotatingCamTransform != null) {
            foundCam = true;
        }

        RemoveSceneCanvas();
    }

    void LateUpdate() {
        if (foundCam) {
            rotatingCamTransform.Rotate(0f, rotatingSpeed * Time.unscaledDeltaTime, 0f, Space.World);
        }
    }

    void RemoveSceneCanvas() {
        Canvas[] canvases = FindObjectsOfType<Canvas>();

        foreach (Canvas canvas in canvases) {
            if (canvas != GetComponent<Canvas>()) {
                canvas.gameObject.SetActive(false);
            }
        }
    }

    void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

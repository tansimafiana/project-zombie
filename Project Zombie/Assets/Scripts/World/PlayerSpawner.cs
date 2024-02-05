using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private GameObject[] UI_Elements;

    private GameObject insCanvas;

    [SerializeField] private float spawnCooldown = 0.2f;
    bool canSpawn = true;


    void Start() {
        SceneChanger.IsSpectating += DisableSpawning;
        SceneChanger.IsMultiplayer += DisableSpawning;
        SceneChanger.IsSingleplayer += SpawnProtocol;
    }

    void DisableSpawning() {
        canSpawn = false;
        this.enabled = false;
    }

    void InitPlayer() {
        //Instantiate(playerPrefab, transform.position, Quaternion.identity);
        NetworkManager.Singleton.StartHost();
    }

    void InitCanvas() {
        insCanvas = Instantiate(mainCanvas);
        InitUI();
    }

    void InitUI() {
        foreach (Transform child in insCanvas.transform) {
            Destroy(child.gameObject);
        }

        foreach (GameObject element in UI_Elements) {
            Instantiate(element, insCanvas.transform);
            element.transform.SetParent(insCanvas.transform, false);
        }
    }

    public void SpawnProtocol() {
        Debug.Log("[PlayerSpawner] SpawnProtocol!");
        if (canSpawn) {
            SceneChanger.SignalPlayMode();
            DisableAllCameras();

            if (Object.FindObjectOfType<PlayerStats>() == null) {
                InitPlayer();
            }

            bool canSpawnCanvas = true;
            foreach (Canvas canvas in Object.FindObjectsOfType<Canvas>()) {
                if (canvas.gameObject.tag == "MainCanvas") {
                    insCanvas = canvas.gameObject;
                    canSpawnCanvas = false;
                    InitUI();
                }
            }
            if (canSpawnCanvas) {
                InitCanvas();
            }

            //FindObjectOfType<DirectorZombieSpawner>().RoundInitProtocol();
        }
    }

    void DisableAllCameras() {
        Camera[] cameras = FindObjectsOfType<Camera>();
        foreach (Camera cam in cameras) {
            if (cam.gameObject.tag != "MainCamera" && cam.gameObject.name != "UI-Camera") {
                cam.gameObject.SetActive(false);
            }
        }
    }
}

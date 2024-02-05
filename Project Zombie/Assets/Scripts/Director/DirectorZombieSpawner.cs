using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DirectorZombieSpawner : NetworkBehaviour
{
    public Transform playerTransform;

    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] EnemySO[] zombieTypes;
    [Space]
    [SerializeField] private int currWave = 1;
    [SerializeField] private int _activeZombiesInScene = 0;
    public int ActiveZombiesInScene {
        get { return _activeZombiesInScene; }
        set { _activeZombiesInScene = value; 
                if (value <= 0) {
                    StartNewWave();
                }
            }
    }
    [Space]
    [SerializeField] private float timeBtwWaves = 5f;

    List<ZombieSpawnerBehaviour> spawners = new List<ZombieSpawnerBehaviour>();
    public bool spawnersActive = true;      // Each wave this will be set to false. If it remains false when spawners should be spawning, it means the last spawner has
                                            //  spawned its zombie and the game has finished.


    public static DirectorZombieSpawner Instance { get; private set; }


    [SerializeField] GameObject winScreenUI;
    public Transform mainCanvasTransform;

    void Awake() {
        // If there's an instance, and it's not myself, delete myself
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }


        // Caching the zombie spawners in the scene
        foreach (ZombieSpawnerBehaviour spawner in FindObjectsOfType<ZombieSpawnerBehaviour>()) {
            spawners.Add(spawner);
        }

        Canvas[] cs = FindObjectsOfType<Canvas>();;
        foreach (Canvas c in cs) {
            if (c.CompareTag("MainCanvas")) {
                mainCanvasTransform = c.transform;
            }
        }

        BeginGame();
    }

    public void BeginGame() {
        currWave = 0;
        StartNewWave();
    }

    // We increment the wave counter, and begin the coroutine for the new wave
    public void StartNewWave() {
        spawnersActive = false;
        StartCoroutine(DelayNewWave());
    }

    IEnumerator DelayNewWave() {
        yield return new WaitForSeconds(timeBtwWaves);
        Debug.Log("<color=grey>New Wave!</color>");

        currWave++;
        SpawnFromZombSpawners();
    }

    void SpawnFromZombSpawners() {
        foreach (ZombieSpawnerBehaviour spawner in spawners) {
            spawner.SpawnZombies(zombiePrefab, zombieTypes, currWave);
        }

        EndOfGameCheck();
    }

    void EndOfGameCheck() {
        if (!spawnersActive) {      // No spawners are active, commence game win.
            Debug.Log("<color=yellow>You won!</color>");
            
            //SceneManager.LoadScene("Lobby Menu");
            //RoundCurrencyManager.SendPlayerBack();
            ShowWinScreen();
        }
    }

    void ShowWinScreen() {
        Cursor.lockState = CursorLockMode.None;

        Instantiate(winScreenUI, mainCanvasTransform);
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryHolder : MonoBehaviour
{
    public GunSO primaryEquip;
    public GunSO secondaryEquip;
    public GunSO meleeEquip;

    public Texture playerSkin;
    public HatSO playerHat;
    public bool isSingleplayer = true;
    [SerializeField] float bufferCheckTime = 1f;

    string currLoadedScene = "";

    void Start() {
        InventoryHolder[] holders = FindObjectsOfType<InventoryHolder>();

        foreach (InventoryHolder h in holders) {
            if (h != this) {
                Destroy(gameObject);
            } else {
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name != "Pause Scene") {
            Debug.Log("[InventoryHolder]   Scene: " + scene.name + "\tMode: " + mode);
            currLoadedScene = scene.name;
        
            if (scene.name != "Lobby Menu")  { StartCoroutine(BufferModeCheck()); }
        }
    }

    void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        StopAllCoroutines();
    }

    void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        StopAllCoroutines();
    }

    IEnumerator BufferModeCheck() {
        yield return new WaitForSeconds(bufferCheckTime);

        if (isSingleplayer) {
            SceneChanger.SignalSingleplayer();
        } else {
            SceneChanger.SignalMultiplayer();
        }
    }
}

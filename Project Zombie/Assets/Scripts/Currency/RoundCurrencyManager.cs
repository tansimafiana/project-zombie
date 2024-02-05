using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class RoundCurrencyManager : MonoBehaviour
{
    public static void SendPlayerBack(PlayerStats stats = null, string sceneName = "Lobby Menu") {
        PlayerPrefs.SetInt("commons_killed", (stats == null) ? 0 : stats.CommonsKilledInMatch);
        PlayerPrefs.SetInt("fasts_killed", (stats == null) ? 0 : stats.FastsKilledInMatch);
        PlayerPrefs.SetInt("brutes_killed", (stats == null) ? 0 : stats.BrutesKilledInMatch);
        PlayerPrefs.SetInt("round_complete", 1);

        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in canvases) {
            if (canvas.gameObject.tag == "MainCanvas") {
                Destroy(canvas.gameObject);
            }
        }
        Camera[] cameras = FindObjectsOfType<Camera>();
        foreach (Camera cam in cameras) {
            if (cam.gameObject.tag == "MainCamera") {
                Destroy(cam.gameObject);
            }
        }
        if (stats != null) {
            Destroy(FindObjectOfType<CinemachineFreeLook>().gameObject);
            Destroy(stats.gameObject);
        } else {
            stats = FindObjectOfType<PlayerStats>();
            if (stats) {
                Destroy(stats.gameObject);
            }
        }
        Destroy(FindObjectOfType<NetworkManager>().gameObject);

        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(sceneName);
    }
}

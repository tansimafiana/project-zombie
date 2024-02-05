using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPauseBehaviour : MonoBehaviour
{
    bool isPaused = false;


    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) {
            Un_PauseProtocol();
        }
    }

    public void Un_PauseProtocol() {
        isPaused = !isPaused;

        if (isPaused) {
            SceneManager.UnloadSceneAsync("Pause Scene");
            Time.timeScale = 1;
        } else {
            SceneManager.LoadSceneAsync("Pause Scene", LoadSceneMode.Additive);
            Time.timeScale = 0;
        }
    }
}

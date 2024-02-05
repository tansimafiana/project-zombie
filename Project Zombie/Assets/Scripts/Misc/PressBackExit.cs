using UnityEngine;
using UnityEngine.SceneManagement;

public class PressBackExit : MonoBehaviour
{
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            RoundCurrencyManager.SendPlayerBack(sceneName: "Start Menu");
        }
    }
}

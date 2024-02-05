using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneChanger : MonoBehaviour
{
    public delegate void SceneChangeAction();
    public static event SceneChangeAction IsSpectating;
    public static event SceneChangeAction IsPlaying;
    public static event SceneChangeAction IsMultiplayer;
    public static event SceneChangeAction IsSingleplayer;

    bool isSingleplayer = true;


    public static void SignalSpectatorMode() {
        if (IsSpectating != null) {
            IsSpectating();
            Debug.Log("<color=lime>Scene is Spectating!</color>");
        }
    }

    public static void SignalPlayMode() {
        if (IsPlaying != null) {
            IsPlaying();
            Debug.Log("<color=lime>Scene is Playing!</color>");
        }
    }

    public static void SignalMultiplayer() {
        if (IsMultiplayer != null) {
            IsMultiplayer();
            Debug.Log("<color=orange>Multiplayer Mode!</color>");
        }
    }

    public static void SignalSingleplayer() {
        if (IsSingleplayer != null) {
            IsSingleplayer();
            Debug.Log("<color=orange>Singleplayer Mode!</color>");
        }
    }

    public void ChangeFromStartMenu(string newSceneName) {
        FindObjectOfType<InventoryHolder>().isSingleplayer = this.isSingleplayer;

        SceneManager.LoadScene(newSceneName);
    }

    public void Exit() {
        Application.Quit();
    }


    public void SwitchSingleMulti(TextMeshProUGUI text) {
        isSingleplayer = !isSingleplayer;

        if (isSingleplayer) {
            text.text = "Singleplayer";
        } else {
            text.text = "Multiplayer";
        }
    }
}

using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] Transform serverFunctionBtnPrefab;
    
    void Start() {
        Transform canvas = Object.FindObjectOfType<Canvas>().transform;

        RectTransform serverButton = Instantiate(serverFunctionBtnPrefab, canvas).GetComponent<RectTransform>();
        RectTransform hostButton   = Instantiate(serverFunctionBtnPrefab, canvas).GetComponent<RectTransform>();
        RectTransform clientButton = Instantiate(serverFunctionBtnPrefab, canvas).GetComponent<RectTransform>();

        hostButton.anchoredPosition   -= (serverButton.sizeDelta.y + 10f) * Vector2.up;
        clientButton.anchoredPosition -= ((serverButton.sizeDelta.y * 2f) + 10f) * Vector2.up;

        serverButton.GetComponentInChildren<TextMeshProUGUI>().text = "Server";
        hostButton.GetComponentInChildren<TextMeshProUGUI>().text   = "Host";
        clientButton.GetComponentInChildren<TextMeshProUGUI>().text = "Client";


        serverButton.GetComponent<Button>().onClick.AddListener(() => {
            Debug.Log("<color=lime>Started Server!</color>");
            NetworkManager.Singleton.StartServer();

            DisableSceneCameras();
        });
        hostButton.GetComponent<Button>().onClick.AddListener(() => {
            Debug.Log("<color=lime>Starting Host!</color>");
            NetworkManager.Singleton.StartHost();

            //FindObjectOfType<DirectorZombieSpawner>().RoundInitProtocol();      // Doing a 'FindObjectOfType' might screw up multiplayer sync in the future
            DisableSceneCameras();
        });
        clientButton.GetComponent<Button>().onClick.AddListener(() => {
            Debug.Log("<color=lime>Starting Client!</color>");
            NetworkManager.Singleton.StartClient();

            DisableSceneCameras();
        });
    }

    void DisableSceneCameras() {
        foreach (Camera cam in FindObjectsOfType<Camera>()) {
            if (cam.gameObject.name == "UI-Camera")  { continue; }
            Debug.Log(cam.gameObject.name);
            cam.gameObject.SetActive(false);
        }
    }
}

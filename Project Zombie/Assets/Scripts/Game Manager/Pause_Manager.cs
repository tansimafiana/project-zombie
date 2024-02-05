using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Pause_Manager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    private Transform mainCanvas;
    private TextMeshProUGUI sensitivityLabel;

    private CameraWork cameraWork;

    bool isPaused = false;

    void Start() {
        StartCoroutine(Initialize());
    }

    void LateUpdate() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            isPaused = !isPaused;
            pausePanel.SetActive(isPaused);
        }
    }

    IEnumerator Initialize() {
        yield return new WaitForSeconds(0.5f);
        mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas").transform;
        cameraWork = FindObjectOfType<CameraWork>();

        pausePanel = Instantiate(pausePanel, mainCanvas);
        
        Slider sensitivitySlider = pausePanel.GetComponentInChildren<Slider>();
        sensitivitySlider.onValueChanged.AddListener(delegate { UpdateSensitivity(sensitivitySlider.value); });
        sensitivityLabel = sensitivitySlider.GetComponentInChildren<TextMeshProUGUI>();

        Button[] buttons = pausePanel.GetComponentsInChildren<Button>();
        foreach (Button b in buttons) {
            if      (b.name == "Back Button")  { b.onClick.AddListener(delegate() { UnPause(); }); }
            else if (b.name == "Exit Button")  { b.onClick.AddListener(delegate() { LoadLobby(); }); }
        }

        pausePanel.SetActive(false);
    }

    void UpdateSensitivity(float value) {
        cameraWork.sensitivity = value * 0.01f;
        sensitivityLabel.text = string.Concat("Sensitivity:   ", value.ToString("0.0"));
    }

    void UnPause() {
        pausePanel.SetActive(false);
        isPaused = false;
    }

    void LoadLobby() {
        RoundCurrencyManager.SendPlayerBack();
    }
}

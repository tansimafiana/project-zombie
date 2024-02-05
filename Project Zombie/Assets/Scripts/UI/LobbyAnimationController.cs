using UnityEngine;
using UnityEngine.UI;

public class LobbyAnimationController : MonoBehaviour
{
    private Vector2 canvasScale;
    [SerializeField] private float panelPaddings = 20f;
    [Space]
    [SerializeField] private RectTransform t_levelPanel;
    [SerializeField] private RectTransform t_shopPanel;
    [SerializeField] private RectTransform t_levelButton;
    [SerializeField] private RectTransform t_shopButton;
    [SerializeField] RectTransform t_characterPanel;
    [SerializeField] RectTransform t_characterButton;

    void Start() {
        canvasScale = GetComponent<RectTransform>().sizeDelta;

        t_levelPanel.anchoredPosition = new Vector2(-canvasScale.x, 0f);
        t_levelPanel.sizeDelta = new Vector2(canvasScale.x - panelPaddings * 2f, canvasScale.y - panelPaddings * 2f);
        t_levelPanel.gameObject.SetActive(false);

        t_shopPanel.anchoredPosition = new Vector2(canvasScale.x, 0f);
        t_shopPanel.sizeDelta = new Vector2(canvasScale.x - panelPaddings * 2f, canvasScale.y - panelPaddings * 2f);
        t_shopPanel.gameObject.SetActive(false);

        t_characterPanel.anchoredPosition = new Vector2(0f, canvasScale.y);
        t_characterPanel.sizeDelta = new Vector2(canvasScale.x - panelPaddings * 2f, canvasScale.y - panelPaddings * 2f);
        t_characterPanel.gameObject.SetActive(false);
    }

    public void OpenLevelSelect() {
        t_levelPanel.gameObject.SetActive(true);
        t_shopButton.gameObject.SetActive(false);
        t_levelButton.gameObject.SetActive(false);
        t_characterButton.gameObject.SetActive(false);
        t_levelPanel.anchoredPosition = Vector2.zero;
    }

    public void CloseLevelSelect() {
        t_levelPanel.anchoredPosition = new Vector2(-canvasScale.x, 0f);
        t_levelButton.gameObject.SetActive(true);
        t_levelPanel.gameObject.SetActive(false);
        t_shopButton.gameObject.SetActive(true);
        t_characterButton.gameObject.SetActive(true);
    }

    public void OpenShopSelect() {
        t_shopPanel.gameObject.SetActive(true);
        t_shopButton.gameObject.SetActive(false);
        t_levelButton.gameObject.SetActive(false);
        t_characterButton.gameObject.SetActive(false);
        t_shopPanel.anchoredPosition = Vector2.zero;
    }

    public void CloseShopSelect() {
        t_shopPanel.anchoredPosition = new Vector2(canvasScale.x, 0f);
        t_shopPanel.gameObject.SetActive(false);
        t_shopButton.gameObject.SetActive(true);
        t_levelButton.gameObject.SetActive(true);
        t_characterButton.gameObject.SetActive(true);
    }

    public void OpenCharacterSelect() {
        t_levelButton.gameObject.SetActive(false);
        t_shopButton.gameObject.SetActive(false);
        t_characterButton.gameObject.SetActive(false);
        t_characterPanel.gameObject.SetActive(true);
        t_characterPanel.anchoredPosition = Vector2.zero;
    }

    public void CloseCharacterSelect() {
        t_levelButton.gameObject.SetActive(true);
        t_shopButton.gameObject.SetActive(true);
        t_characterButton.gameObject.SetActive(true);
        t_characterPanel.gameObject.SetActive(false);
        t_characterPanel.anchoredPosition = new Vector2(0f, canvasScale.y);
    }
}

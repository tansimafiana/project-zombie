using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SelectionPanelEntityBehaviour : MonoBehaviour
{
    // We want to know the current GameObject we're outlining, and we want to receive the next GameObject that was selected.
    // After we got them two, we tween from one to the other.
    // We will tween the poisiton and the scale.

    private GameObject currSelection;
    private GameObject targetSelection;
    private Transform necessitiesStorageTransform;
    [SerializeField] private float xDir;
    [SerializeField] private float yDir;

    public static SelectionPanelEntityBehaviour Instance;

    void Start() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
            necessitiesStorageTransform = transform.parent;
        }
    }

    public void SelectTarget(GameObject target) {
        if (currSelection == null)  { InitializeSelection(target); return; }

        RectTransform rectTransform = GetComponent<RectTransform>();
        transform.SetParent(target.transform);
        currSelection = target;

        transform.DOMove(target.transform.position + new Vector3(xDir, yDir), 1f).SetEase(Ease.InOutQuad);
        transform.SetParent(necessitiesStorageTransform);
    } 

    private void InitializeSelection(GameObject target) {
        // 'Blow' up from the origin using scale
        
        RectTransform rectTransform = GetComponent<RectTransform>();
        transform.SetParent(target.transform);

        rectTransform.offsetMin = new Vector2(75f, -75f);
        rectTransform.offsetMax = new Vector2(75f, -75f);
        rectTransform.pivot     = Vector2.right;

        // rectTransform.sizeDelta = transform.parent.GetComponent<RectTransform>().sizeDelta + Vector2.one * 10f;
        rectTransform.sizeDelta = Vector2.one * 15f;

        rectTransform.localScale = Vector3.zero;

        DOTween.To(() => rectTransform.localScale, x => rectTransform.localScale = x, new Vector3(1f, 1f,  1f), 0.5f)
            .SetEase(Ease.InOutQuad);

        transform.SetParent(necessitiesStorageTransform);
        currSelection = target;
    }

    void OnDisable() {
        SelectionPanelEntityBehaviour.Instance = null;
    }
}

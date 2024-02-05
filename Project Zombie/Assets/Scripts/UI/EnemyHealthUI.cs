using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    [SerializeField] private Camera cam;
    public Slider slider;
    [SerializeField] private Transform canvasTransform;

    [SerializeField] float timeToFade = 3f;

    void Start() {
        if (cam == null) {
        //    Debug.Log("[EnemyHealthUI] 'cam' is NULL");
            cam = Camera.main;
        }

        //slider = transform.GetComponentInChildren<Slider>();

        slider.gameObject.SetActive(false);
    }

    
    /*void LateUpdate() {
        canvasTransform.LookAt(cam.transform);
        canvasTransform.localEulerAngles = new Vector3(0f, canvasTransform.localEulerAngles.y, 0f);
    }*/
    
    public void UpdateSlider(float newValue, bool showHealth) {
        slider.value = newValue;

        this.StopAllCoroutines();
        if (showHealth)  { StartCoroutine(SliderDissipate()); }
    }

    public void SetSlider(float maxHealth) {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
    }

    IEnumerator SliderDissipate() {
        slider.gameObject.SetActive(true);
        yield return new WaitForSeconds(timeToFade);
        slider.gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairBehaviour : MonoBehaviour
{
    [SerializeField] private float idleRadius;

    private float rotSpeed;
    [Space]
    [SerializeField] private float idleRotSpeed;
    [SerializeField] private float targetRotSpeed;

    private bool _isTargeting;
    public bool IsTargeting {
        get { return _isTargeting; }
        set {
            _isTargeting = value;
            
            rotSpeed = (_isTargeting) ? targetRotSpeed : idleRotSpeed;
            ReColourArrows((_isTargeting) ? targetColour : idleColour);
        }
    }

    private bool hasHitSomething = false;

    [Space]
    [SerializeField] private Color idleColour;
    [SerializeField] private Color targetColour;
    [SerializeField] private Color hitColour;
    [SerializeField] private Color headshotColour;

    [Space]
    [SerializeField] private float timeHoldingHitColour = 0.2f;

    private Transform[] arrowTransforms = new Transform[4];


    void Start() {
        rotSpeed = idleRotSpeed;

        // Crosshair has 4 arrows for now, could change in the future
        for (int i = 0; i < 4; i++) {
            arrowTransforms[i] = transform.GetChild(i);     // Order goes like:     North, East, South, West

            arrowTransforms[i].localEulerAngles = (((float)i * -90f) + 180f) * Vector3.forward;     // Start from 180, go to -90.
        }

        arrowTransforms[0].localPosition = Vector2.up    * idleRadius;
        arrowTransforms[1].localPosition = Vector2.right * idleRadius;
        arrowTransforms[2].localPosition = Vector2.down  * idleRadius;
        arrowTransforms[3].localPosition = Vector2.left  * idleRadius;

        ReColourArrows(idleColour);


        // Hacky hack hack ^-^
        Shoot shoot = Object.FindObjectOfType<Shoot>();
        shoot.crosshairBehaviour = this;
        shoot.GetComponent<ShootDesktop>().crosshairBehaviour = this;
    }

    void Update() {
        transform.Rotate(rotSpeed * Vector3.forward * Time.deltaTime);
    }

    private void ReColourArrows(Color colour) {
        if (!hasHitSomething) {
            foreach (Transform arrow in arrowTransforms) {
                arrow.GetComponent<RawImage>().color = colour;
            }
        }
    }

    public void HitProcedure() {
        StopAllCoroutines();
        StartCoroutine(ColourFlashTiming(hitColour));
    }

    public void HeadHitProcedure() {
        StopAllCoroutines();
        StartCoroutine(ColourFlashTiming(headshotColour));
    }

    IEnumerator ColourFlashTiming(Color colour) {
        ReColourArrows(colour);
        hasHitSomething = true;

        yield return new WaitForSeconds(timeHoldingHitColour);

        hasHitSomething = false;
        ReColourArrows(targetColour);
    }

}

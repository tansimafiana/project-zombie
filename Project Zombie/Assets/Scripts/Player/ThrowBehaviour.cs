using UnityEngine;

public class ThrowBehaviour : MonoBehaviour
{
    public Transform t_cam;
    [Space]
    [SerializeField] private ThrowSO throwable;
    [SerializeField] private float throwForce;

    private int lastSlotUsed = 0;


    void Update() {
        if (Input.GetKeyDown(KeyCode.G)) {
            lastSlotUsed = GetComponent<PlayerInventory>().DisableWeapons();
            EnableChargingAnimation();
        } else if (Input.GetKeyUp(KeyCode.G)) {
            Throw();
            GetComponent<PlayerInventory>().EnableWeapon(lastSlotUsed);
            DisableChargingAnimation();
        }
    }

    public void Throw() {
        Transform proj = Instantiate(throwable.prefab).transform;
        proj.position = transform.position;
        proj.forward = t_cam.forward;
        proj.localScale = throwable.size;

        Rigidbody rb = proj.gameObject.AddComponent<Rigidbody>();
        rb.AddForce(throwForce * proj.forward);

        proj.gameObject.AddComponent<ThrowableBehaviour>().Init(GetComponent<PlayerStats>(),
                                                                throwable.damage,
                                                                throwable.activationTime,
                                                                throwable.effectRadius,
                                                                throwable.stunPercentSpeed,
                                                                throwable.stunTime,
                                                                throwable.minPushbackForce,
                                                                throwable.maxPushbackForce,
                                                                throwable.particles);
    }

    void EnableChargingAnimation() {
        Animator p_animator = GetComponent<Animator>();
        p_animator.SetLayerWeight(1, 0f);       // Setting Gun Aiming layer to 0
    }

    void DisableChargingAnimation() {
        Animator p_animator = GetComponent<Animator>();
        p_animator.SetLayerWeight(1, 1f);       // Setting Gun Aiming layer to 1
    }
}

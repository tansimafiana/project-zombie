using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    public GunSO gun;
    private Transform gunModel;
    [SerializeField] private Vector3 floatingPos;
    [SerializeField] private float amplitude = 1f;
    [SerializeField] private float speed = 1f;

    private bool hasWeapon = true;

    void Start() {
        gunModel = Instantiate(gun.prefab).transform;

        gun.handheldPos = gunModel.localPosition;
        gun.handheldRot = gunModel.localEulerAngles;

        gunModel.SetParent(transform);
        gunModel.localPosition = floatingPos;

        AudioSource audioSource = gunModel.gameObject.AddComponent<AudioSource>();
        audioSource.clip = gun.s_gunshot;
        audioSource.volume = gun.s_volume;
        audioSource.spatialBlend = 1f;
    }

    void LateUpdate() {
        if (hasWeapon) { 
            gunModel.localPosition = floatingPos + Vector3.up * Mathf.Sin(Time.time * speed) * amplitude;
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.transform.parent != null) {
            if (other.transform.parent.tag == "Player") {
                GiveWeapon(other.transform.parent.Find("Model"));
            }
        }
    }

    void GiveWeapon(Transform parent) {
        // Instead of destroying the gun and instantiating a new one, we'll just transfer ownership to the player
        hasWeapon = false;
        gunModel.SetParent(parent);

        gunModel.localPosition = gun.handheldPos;
        gunModel.localEulerAngles = gun.handheldRot;
        Debug.Log(gun.handheldPos);

        parent.GetComponentInParent<Shoot>().AttachWeapon(gun, gunModel, gun.clipSize, gun.ammoAmount);
        //parent.GetComponentInParent<ShootDesktop>().AttachWeapon(gun, gunModel);

        Animator p_animator = parent.GetComponentInParent<Animator>();
        p_animator.SetLayerWeight(1, 1f);

        gunModel.gameObject.layer = LayerMask.NameToLayer("Player");

        this.gunModel = null;
    }
}

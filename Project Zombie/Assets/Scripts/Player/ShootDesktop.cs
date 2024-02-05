using UnityEngine;

public class ShootDesktop : Shoot
{
    void Start() {
        MobileCheck.CheckForMobile += DisableIfMobile;
        MobileCheck.CheckForDesktop += InitializeAssets;

        MobileCheck.SwitchPlatforms();
    }

    void Update() {
        if (!IsOwner)  return;

        this.ShootTargetingRay(this.aimRange);

        if (Input.GetMouseButtonDown(0))     { FirePressedDown(); }
        else if (Input.GetMouseButtonUp(0))  { FirePressedUp(); }
        this.timeTillNextShot -= Time.deltaTime;    // Counting down


        // Actually Firing
        if (this.isShooting) {
            if (this.equippedWeapon.isAuto) {
                Fire();
            } else {
                Fire();
                FirePressedUp();
            }
        }

        // Zooming in and out
        /*if (Input.GetMouseButtonDown(1)) {
            GetComponent<CameraWork>().thirdPersonCam.m_Lens.FieldOfView = 40f;
            this.isAiming = true;
            SignalRotate();
        } else if (Input.GetMouseButtonUp(1)) {
            GetComponent<CameraWork>().thirdPersonCam.m_Lens.FieldOfView = 90f;
            this.isAiming = false;
            StopRotate();
        }*/

        if (Input.GetKeyDown(KeyCode.R))  { Reload(); }
    }

    void InitializeAssets() {
        Debug.Log("[Shoot] " + gameObject.name);
        Shoot shoot = GetComponent<Shoot>();

        this.cam = shoot.cam;
        this.modelTransform = shoot.modelTransform;
        this.whatToHit = shoot.whatToHit;
        this.equippedWeapon = shoot.equippedWeapon;
        this.equippedWeaponAudio = shoot.equippedWeaponAudio;
        this.particles = shoot.particles;
        this.clipAmount = shoot.clipAmount;
        this.reserveAmount = shoot.reserveAmount;
        this.animator = GetComponent<Animator>();
        this.objectPooler = ObjectPooler.Instance;
        this.ammoLabel = shoot.ammoLabel;
        this.crosshairBehaviour = shoot.crosshairBehaviour;
        this.aimRange = (this.equippedWeapon == null) ? this.aimRange : this.equippedWeapon.range;

        Destroy(shoot);
    }

    void DisableIfMobile() {
        Destroy(this);    
    }

    void OnDisable() {
        MobileCheck.CheckForMobile -= DisableIfMobile;
        MobileCheck.CheckForDesktop -= InitializeAssets;
    }
}

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;

public class Shoot : NetworkBehaviour
{
    public Camera cam;
    public GunSO equippedWeapon;
    [HideInInspector] public AudioSource equippedWeaponAudio;
    [HideInInspector] public ParticleSystem particles;
    public Transform modelTransform;

    [SerializeField] GameObject ammoLabelPrefab;
    public TextMeshProUGUI ammoLabel;
    public int clipAmount;
    public int reserveAmount;

    protected bool isAiming = false;
    [Space]
    [SerializeField] protected bool isShooting = false;
    [SerializeField] protected float timeTillNextShot = 0f;
    protected float aimRange = 5f;
    [Space]
    public LayerMask whatToHit;

    [Space]
    [SerializeField] GameObject bloodParticles;
    [SerializeField] GameObject bulletHitParticles;
    [SerializeField] GameObject gunshotPrefab;
    [Space]
    //[SerializeField] int maxActiveBulletholes = 25;
    //BulletholeID[] arr_activeBulletholes;
    protected ObjectPooler objectPooler;


    // DEBUG
    List<Vector3[]> debugArcPositions = new List<Vector3[]>();
    protected Animator animator;

    public CrosshairBehaviour crosshairBehaviour;
    private Collider prevCollider = null;

    StringBuilder sb = new StringBuilder();

    
    void Start() {
        if (!IsOwner)  return;

        if (cam == null) {
            Debug.Log("[Shoot] 'cam' is NULL, using unsafe method");
            cam = Camera.main;
        }

        animator = GetComponent<Animator>();
        objectPooler = ObjectPooler.Instance;
        /*arr_activeBulletholes = new BulletholeID[maxActiveBulletholes];
        for (int i = 0; i < maxActiveBulletholes; i++) {
            arr_activeBulletholes[i] = new BulletholeID();
            arr_activeBulletholes[i].ID = -1;
        }*/

        Canvas[] canvas = FindObjectsOfType<Canvas>();
        foreach (Canvas c in canvas) {
            if (c.gameObject.tag == "MainCanvas") {
                GameObject label = Instantiate(ammoLabelPrefab, c.transform);
                ammoLabel = label.GetComponentInChildren<TextMeshProUGUI>();
                crosshairBehaviour = c.GetComponentInChildren<CrosshairBehaviour>();
                Debug.Log("<color=orange>[Shoot] Found the MainCanvas!</color>");
            }
        }

        Debug.Log("[Shoot] Initialized!");
    }

    void Update() {
        if (!IsOwner)  return;

        ShootTargetingRay(aimRange);

        if (isShooting) {
            if (equippedWeapon.isAuto) {
                Fire();
            } else {
                Fire();
                FirePressedUp();
            }
        }

        // if (Input.GetKeyDown(KeyCode.R))  { Reload(); }

        timeTillNextShot -= Time.deltaTime;
    }

    public void Fire() {
        if (timeTillNextShot <= 0f) {
            if (equippedWeapon.isMelee) {
                SwingMelee(aimRange, equippedWeapon.damage);
                timeTillNextShot = equippedWeapon.timeBtwShots;
            } else {
                if (clipAmount > 0) {
                    if (equippedWeapon.isShotgun) {
                        clipAmount += equippedWeapon.shotgunPellets - 1;
                        ShootShotgun(equippedWeapon.shotgunPellets, aimRange,
                                    equippedWeapon.damage, equippedWeapon.maxSpread);
                    } else {
                        ShootRay(aimRange, equippedWeapon.damage);
                    }

                    try {
                        particles.Play();
                        if (equippedWeaponAudio.clip != null)  { equippedWeaponAudio.PlayOneShot(equippedWeaponAudio.clip); }
                    }
                    catch (NullReferenceException) {}

                    timeTillNextShot = equippedWeapon.timeBtwShots;
                }
            }
        }
    }

    public void FirePressedDown()  {
        if (equippedWeapon != null) {
            isShooting = true;
            isAiming = true;
            SignalRotate();

            try { equippedWeaponAudio.volume = equippedWeapon.s_volume; }
            catch (NullReferenceException) {}
        }
    }
    public void FirePressedUp()  {
        isShooting = false;
        StopRotate();

        StartCoroutine(StopParticles());
    }

    void ShootRay(float range = 1f, float damage = 1f, Vector2 xyOffset = new Vector2()) {
        //Vector3 dir = (camAnchor.position - cam.transform.position).normalized
        Ray ray = new Ray(cam.transform.position, cam.transform.forward + (Vector3)xyOffset);
        Debug.DrawRay(ray.origin, ray.direction * range, Color.blue, 3f);

        // Variables for determening blood splatter positions and which zombie got hit first.
        Collider closestCollider = null;
        float closestPos = Mathf.Infinity;
        Vector3 hitPos = new Vector3();
        Vector3 hitNormal = new Vector3();

        RaycastHit[] hitData = Physics.RaycastAll(ray, range, whatToHit);
        foreach (RaycastHit hit in hitData) {

            float dist = Vector3.Distance(transform.position, hit.point);
            if (dist < closestPos && hit.collider.tag != "ExplosionCol") {
                closestPos = dist;
                hitPos = hit.point;
                hitNormal = hit.normal;
                closestCollider = hit.collider;
            }
        }
        if (closestCollider) {
            // Now we do a check if it's an enemy or another object
            EnemyStats stats = closestCollider.GetComponentInParent<EnemyStats>();
            PetDeath pet = closestCollider.GetComponent<PetDeath>();
            if (stats) {
                // We hit an enemy
                if (closestCollider.name == "Head Collider") {
                    damage *= 2.5f;
                    crosshairBehaviour.HeadHitProcedure();
                } else {
                    crosshairBehaviour.HitProcedure();
                }

                damage = (closestCollider.name == "Head Collider") ? damage * 2.5f : damage;
                bool killingShot;
                stats.Damage(damage, out killingShot);

                if (killingShot) {
                    PlayerStats p_stats = GetComponent<PlayerStats>();
                    if      (stats.name == "Common")  { p_stats.CommonsKilledInMatch++; }
                    else if (stats.name == "Fast")    { p_stats.FastsKilledInMatch++; }
                    else if (stats.name == "Brute")   { p_stats.BrutesKilledInMatch++; }
                    else                              { p_stats.CommonsKilledInMatch++; }
                }
                
                //BloodSplatter(hitPos, 3f);
                SpawnPooledEffect("blood_spray", hitPos, hitNormal);
            } else if (pet) {
                pet.Bleh();
            } else {
                // We hit a ground object
                //BulletHitParticles(hitPos, 0.1f);
                //CreateGunshot(hitPos, hitNormal, 60f);
                SpawnPooledEffect("bullet_hit", hitPos, hitNormal);
                SpawnPooledEffect("bullet_hole", hitPos, hitNormal);
            }
        }

        UpdateAmmo(clipAmount - 1);
    }

    public void ShootTargetingRay(float range = 1f) {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        //Debug.DrawRay(ray.origin, ray.direction * range, Color.blue, 3f);

        // Variables for determening blood splatter positions and which zombie got hit first.
        Collider closestCollider = null;
        float closestPos = Mathf.Infinity;
        Vector3 hitPos = new Vector3();

        RaycastHit[] hitData = Physics.RaycastAll(ray, range, whatToHit);
        foreach (RaycastHit hit in hitData) {

            float dist = Vector3.Distance(transform.position, hit.point);
            if (dist < closestPos) {
                closestPos = dist;
                hitPos = hit.point;
                closestCollider = hit.collider;
            }
        }
        if (closestCollider) {
            if (closestCollider != prevCollider) {
                // Now we do a check if it's an enemy or another object
                EnemyStats stats = closestCollider.GetComponentInParent<EnemyStats>();
                
                //Debug.Log("[Shoot --- ShootTargetingRay] Switched!");
                crosshairBehaviour.IsTargeting = (stats) ? true : false;
            }

            prevCollider = closestCollider;
        }
    }

    void SwingMelee(float range = 1f, float damage = 1f) {
        StartCoroutine(DrawArc(transform.position, modelTransform.forward));
        Collider[] hitData = Physics.OverlapSphere(transform.position, range, whatToHit);
        foreach (Collider col in hitData) {
            if (col.transform.parent != null) {
                if (col.transform.tag == "Enemy") {
                    Vector3 vectorToCollider = (col.transform.position - transform.position).normalized;
                    // 180 degree arc, change 0 to 0.5 for a 90 degree "pie"
                    if (Vector3.Dot(vectorToCollider, modelTransform.forward) > 0) {
                        // Damage enemy
                        bool killingShot;
                        damage = (col.name == "Head Collider") ? damage * 1f : damage;
                        
                        EnemyStats stats = col.GetComponentInParent<EnemyStats>();
                        stats.Damage(damage, out killingShot);

                        if (killingShot) {
                            GetComponent<PlayerStats>().ZombieKilledCount(stats.name);
                        }

                        //BloodSplatter(col.transform.position, 3f);
                        SpawnPooledEffect("blood_spray", col.transform.position, new Vector3(-30f, cam.transform.localEulerAngles.y + 180f, 0f));
                    }
                }
            }
        }
    }

    void ShootShotgun(int pelletAmount, float range, float damage, float maxOffset) {
        for (int i = 0; i < pelletAmount; i++) {
            Vector2 offset = new Vector2(UnityEngine.Random.Range(0f, maxOffset),
                                         UnityEngine.Random.Range(0f, maxOffset));
            ShootRay(range, damage, offset);
        }
    }

    public void Reload() {
        int maxClip = equippedWeapon.clipSize;
        if (reserveAmount > 0 && clipAmount < maxClip) {

            int /*no*/diff = maxClip - clipAmount;
            if (reserveAmount > diff) {
                UpdateAmmo(maxClip, reserveAmount - diff);   // Subtract 'clipSize' from 'reserve'
            } else {
                UpdateAmmo(reserveAmount, 0);                   // Slot all reserve ammo into clip
            }
        }
    }

    public void UpdateAmmo(int clipAmount) {
        this.clipAmount = clipAmount;

        ammoLabel.text = sb.Append(clipAmount).Append(" / ").Append(reserveAmount).ToString();
        sb.Clear();
    }

    public void UpdateAmmo(int clipAmount, int reserveAmount) {
        this.clipAmount = clipAmount;
        this.reserveAmount = reserveAmount;

        ammoLabel.text = sb.Append(clipAmount).Append(" / ").Append(reserveAmount).ToString();
        sb.Clear();
    }

    public void AttachWeapon(GunSO weapon, Transform weaponModel, int clipAmount, int reserveAmount) {
        this.equippedWeapon = weapon;
        aimRange = weapon.range;

        UpdateAmmo(clipAmount, reserveAmount);

        if (weapon.particles != null) {
            Transform particles = Instantiate(weapon.particles).transform;
            particles.SetParent(weaponModel);
            particles.localPosition = weapon.fireLocalPos;
            particles.localEulerAngles = weapon.fireLocalRot;
            
            this.particles = particles.GetComponent<ParticleSystem>();
        }

        this.equippedWeaponAudio = weaponModel.GetComponent<AudioSource>();
    }

    public void SaveAmmoInInv(out int prevClip, out int prevRes) {
        prevClip = this.clipAmount;
        prevRes = this.reserveAmount;
    }


    protected void SignalRotate()  { GetComponent<PlayerMovement>().isAiming = true; }
    protected void StopRotate()  {
        if (!isAiming || !isShooting)
            GetComponent<PlayerMovement>().isAiming = false;
    }

    IEnumerator StopParticles() {
        yield return new WaitForSeconds(0.05f);
        try { particles.Stop(); }
        catch {}
    }

    IEnumerator DrawArc(Vector3 origin, Vector3 forward) {
        Vector3[] points = {origin, forward};
        debugArcPositions.Add(points);
        yield return new WaitForSeconds(3f);
        debugArcPositions.Remove(points);
    }

    void BloodSplatter(Vector3 position, float time) {
        Destroy(Instantiate(bloodParticles, position, Quaternion.Euler(-30f, cam.transform.localEulerAngles.y + 180f, 0f)), time);
    }

    void BulletHitParticles(Vector3 position, float time) {
        //Destroy(Instantiate(bulletHitParticles, position, Quaternion.Euler(0f, cam.transform.localEulerAngles.y + 180f, 0f)), time);
    }

    void CreateGunshot(Vector3 position, Vector3 normal, float time) {
        objectPooler.SpawnFromPool("bullet_hole", position, Quaternion.LookRotation(normal));
    }

    void SpawnPooledEffect(string effect, Vector3 pos, Vector3 normal) {
        objectPooler.SpawnFromPool(effect, pos, Quaternion.LookRotation(normal));
    }

    void OnDrawGizmos() {
        foreach (Vector3[] points in debugArcPositions) {
            Gizmos.color = Color.cyan;
            GizmosExtensions.DrawWireArc(points[0], points[1], 90f, aimRange);
        }
    }
}
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ThrowableBehaviour : MonoBehaviour
{
    private PlayerStats p_thrower;

    private float timeToActivate;
    private float effectRadius;
    private float speedMul;
    private float stunTime;
    private float damage;
    private float minPushbackForce;
    private float maxPushbackForce;
    private GameObject particles;

    private bool gizmoDraw = false;

    public void Init(PlayerStats plWhoThrew, float damage, float activationTime, float effectRadius, float speedMul, float stunTime, float minPushbackForce, float maxPushbackForce, GameObject particles) {
        this.p_thrower = plWhoThrew;
        this.damage = damage;
        this.timeToActivate = activationTime;
        this.effectRadius = effectRadius;
        this.speedMul = speedMul;
        this.stunTime = stunTime;
        this.minPushbackForce = (minPushbackForce == 0f) ? -5000f : minPushbackForce;
        this.maxPushbackForce = (maxPushbackForce == 0f) ? 7500f : maxPushbackForce;
        this.particles = particles;
    }

    void Start() {
        gameObject.AddComponent<SphereCollider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && !other.isTrigger)) {
            Destroy(GetComponent<Rigidbody>());
            StartCoroutine(Activate());
        }
    }

    IEnumerator Activate() {        // TODO: change this from an IEnumerator to a simple function. The stun timings are held in the enemy script.
        yield return new WaitForSeconds(timeToActivate);

        // Exploding
        if (particles != null)  { Instantiate(particles, transform.position, Quaternion.identity); }

        // Turning the object invisible;
        Destroy(GetComponent<SphereCollider>());
        if (TryGetComponent<MeshRenderer>(out MeshRenderer ren)) {
            ren.enabled = false;
        } else if (TryGetComponent<SkinnedMeshRenderer>(out SkinnedMeshRenderer sren)) {
            sren.enabled = false;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, effectRadius);
        foreach (Collider col in hitColliders) {
            if (col.gameObject.tag == "ExplosionCol") {
                EnemyNavmesh mov = col.GetComponentInParent<EnemyNavmesh>();
                if (mov) {
                    mov.StopAllCoroutines();
                    StartCoroutine(mov.Stun(speedMul, stunTime));

                    bool killingHit;
                    mov.GetComponent<EnemyStats>().Damage(damage, out killingHit, minPushbackForce, maxPushbackForce);

                    if (killingHit) {
                        p_thrower.ZombieKilledCount(mov.name);
                    }
                }
            }
        }

        gizmoDraw = true;
        yield return new WaitForSeconds(stunTime);

        Destroy(gameObject);
    }

    void OnDrawGizmos() {
        if (gizmoDraw) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, effectRadius);
        }
    }
}

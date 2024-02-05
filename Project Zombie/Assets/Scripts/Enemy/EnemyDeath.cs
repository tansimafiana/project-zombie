using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EnemyDeath : MonoBehaviour
{
    [SerializeField] private Transform Head;
    [SerializeField] private Transform Body;
    //[SerializeField] private float minPushBackForce = 500f;
    //[SerializeField] private float maxPushBackForce = 1500f;

    bool isDead = false;        // Used for DEBUG
    [SerializeField] Transform armetureTransform;
    [SerializeField] Rigidbody rootRB;
    [SerializeField] Rigidbody headRB;

    void Start() {
        SetActiveRagdoll(false);
    }

    public void DeathProtocol(float minPushbackForce, float maxPushbackForce, Vector3 blastDirection) {
        isDead = true;
        DirectorZombieSpawner.Instance.ActiveZombiesInScene--;

        // Stopping movement
        EnemyNavmesh navMesh = GetComponent<EnemyNavmesh>();
        navMesh.StopAllCoroutines();
        navMesh.stunFX.SetActive(false);
        navMesh.navMeshAgent.enabled = false;
        navMesh.enabled = false;

        // Playing death sound
        EnemySound sound = GetComponent<EnemySound>();
        sound.ASdeath.PlayOneShot(sound.deathSound);
        sound.isDead = true;

        // Doing Ragdoll business
        SetActiveRagdoll(true);
        AddForceOnPart("Head", minPushbackForce, maxPushbackForce, blastDirection);

        StartCoroutine(DisableArmetureCollisions());

        //Destroy(gameObject, 2f);
        StartCoroutine(SeepIntoTheGround());

        try { Destroy(GetComponentInChildren<Slider>().gameObject); } catch {}
    }

    public void SetActiveRagdoll(bool isActive) {
        Collider[] ragdollColliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in ragdollColliders) {
            if (col.gameObject.name == "Head Collider" || 
                col.gameObject.name == "Body Collider" || 
                col.gameObject.name == "Explosion Collider") {
                col.GetComponent<Collider>().enabled = !isActive;
            } else {
                col.GetComponent<Collider>().enabled = isActive;

                Rigidbody rb = col.GetComponent<Rigidbody>();
                rb.detectCollisions = isActive;
                rb.isKinematic = !isActive;
                rb.useGravity = isActive;
                rb.constraints = (isActive) ? RigidbodyConstraints.None : RigidbodyConstraints.FreezeAll;
            }
        }

        if (isActive) {
            GetComponent<Animator>().enabled = false;
        }
    }

    public void AddForceOnPart(string partName, float minPushbackForce, float maxPushbackForce, Vector3 dir) {
        Rigidbody rb;
        if (partName == "Head") {
            rb = headRB;
        } else {
            rb = rootRB;
        }

        rb.AddRelativeForce(Random.Range(minPushbackForce, maxPushbackForce) * dir);
        //Debug.Log("<color=orange>Relative Max Force: " + maxPushbackForce + "</color>");
    }

    void OnDisable() {
        if (!isDead)
            DirectorZombieSpawner.Instance.ActiveZombiesInScene--;
    }

    IEnumerator DisableArmetureCollisions() {
        yield return new WaitForSeconds(5f);
        //Destroy(armetureTransform.gameObject);
        armetureTransform.gameObject.SetActive(false);
    }

    IEnumerator SeepIntoTheGround() {
        float timeToDestroy = 4f;
        
        yield return new WaitForSeconds(5f);
        transform.DOMoveY(transform.position.y - 2f, timeToDestroy);
        yield return new WaitForSeconds(timeToDestroy);
        Destroy(gameObject);
    }
}

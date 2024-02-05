using System.Collections;
using UnityEngine;

public class PetDeath : MonoBehaviour
{
    Transform modelTransform;

    [SerializeField] float turnSpeed = 10f;
    [SerializeField] float deathTime = 5f;
    public GameObject bloodSprayPrefab;
    [SerializeField] ParticleSystem bloodSpray;

    bool isDead = false;
    bool spin = false;

    Quaternion targetRotation = new Quaternion();

    void Start() {
        modelTransform = transform.GetChild(0);
        bloodSpray = Instantiate(bloodSprayPrefab, transform).GetComponent<ParticleSystem>();
    }

    public void Bleh() {
        StartCoroutine(DeathProcess());
    }

    IEnumerator DeathProcess() {
        // We died, so we turn to face belly-up
        spin = true;
        targetRotation = Quaternion.LookRotation(Vector3.up);
        GetComponent<PetFollow>().followPlayer = false;
        GetComponent<SphereCollider>().enabled = false;
        bloodSpray.Play();

        yield return new WaitForSeconds(1f / turnSpeed);
        modelTransform.rotation = Quaternion.LookRotation(Vector3.up);
        spin = false;
        
        yield return new WaitForSeconds(deathTime);     // After this is done, we revive so we face forward.
        spin = true;
        targetRotation = Quaternion.LookRotation(transform.position + Vector3.forward);
        bloodSpray.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        GetComponent<SphereCollider>().enabled = true;

        yield return new WaitForSeconds(1 / turnSpeed);
        spin = false;
        GetComponent<PetFollow>().followPlayer = true;

    }

    void LateUpdate() {
        if (spin) {
            modelTransform.localRotation = Quaternion.Slerp(modelTransform.rotation, targetRotation, Time.unscaledDeltaTime * turnSpeed);
        }
    }
}

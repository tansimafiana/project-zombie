using UnityEngine;

public class WorldInstantiatorMisc : MonoBehaviour
{
    // Starting Points
    public float explosionPower = 1000f;
    public float radius = 5f;

    public ParticleSystem bloodParticles;


    void Start()
    {
        // Loop through each piece and
        //  give the segments Rigidbodies (gravity)
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;

            Rigidbody rb = child.AddComponent<Rigidbody>();
            child.AddComponent<BoxCollider>();

            rb.isKinematic = true;
        }
    }

    // Main Initialization Method    
    public void InstantiateSphere()
    {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.isKinematic = false;
                rb.AddExplosionForce(explosionPower, explosionPos, radius);
            }
        }

        bloodParticles.Play();
    }
}

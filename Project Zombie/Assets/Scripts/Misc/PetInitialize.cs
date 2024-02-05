using UnityEngine;

public class PetInitialize : MonoBehaviour
{
    [SerializeField] GameObject petPrefab;
    [SerializeField] GameObject bloodSprayPrefab;

    void Start() {
        Transform modelParent = new GameObject(petPrefab.name + " Model").transform;
        modelParent.position = transform.position;

        Transform model = Instantiate(petPrefab).transform;
        model.SetParent(modelParent);
        model.localEulerAngles = Vector3.zero;
        model.localScale = Vector3.one * 0.075f;

        PetDeath deathProtocol = modelParent.gameObject.AddComponent<PetDeath>();
        deathProtocol.bloodSprayPrefab = this.bloodSprayPrefab;
        SphereCollider col = modelParent.gameObject.AddComponent<SphereCollider>();
        col.isTrigger = true;

        PetFollow follow = modelParent.gameObject.AddComponent<PetFollow>();
        follow.petAnchor = transform.GetChild(0).Find("PetAnchor");       // This script is attached to the player
        follow.speed = 1.75f;
        follow.rotSpeed = 5f;
        
        IdleFloat idFloat = model.gameObject.AddComponent<IdleFloat>();
        idFloat.speed = 2;
        idFloat.amplitude = 0.1f;
        idFloat.hasOrigin = true;

        Destroy(this);
    }
}

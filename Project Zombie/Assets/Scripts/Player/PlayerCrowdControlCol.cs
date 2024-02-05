using UnityEngine;

public class PlayerCrowdControlCol : MonoBehaviour
{
    [SerializeField] float radius = 3f;

    void Start() {
        SphereCollider col = gameObject.AddComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = radius;
    }

    void OnTriggerEnter(Collider col) {
        EnemyNavmesh nav = col.GetComponentInParent<EnemyNavmesh>();
        if (nav) {
            nav.followOffset *= 0.01f;
        }
    }

    void OnTriggerExit(Collider col) {
        EnemyNavmesh nav = col.GetComponentInParent<EnemyNavmesh>();
        if (nav) {
            nav.followOffset *= 100f;
        }
    }
}

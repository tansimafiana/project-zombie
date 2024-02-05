using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    [HideInInspector] public float currSpeed;
    [SerializeField] private float spottingDistance = 5f;
    SphereCollider radiusCol;

    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform modelTransform;

    [SerializeField] private EnemyHealthUI enemyHealthUI;

    private Animator animator;


    void Start() {
        if (modelTransform == null) {
            Debug.Log("[EnemyMovement] 'modelTransform' is NULL");
        }
        if (enemyHealthUI == null) {
            Debug.Log("[EnemyMovement] 'enemyHealthUI' is NULL");
        }

        currSpeed = moveSpeed;

        radiusCol = gameObject.AddComponent<SphereCollider>();
        radiusCol.isTrigger = true;
        radiusCol.radius = spottingDistance;

        animator = GetComponent<Animator>();
    }

    void Update() {
        if (targetTransform != null) {
            Follow();
        }
    }

    void Follow() {
        Vector3 dirToPlayer = (targetTransform.position - transform.position).normalized;
        Vector3 velocity = dirToPlayer * currSpeed * Time.deltaTime;

        transform.Translate(velocity);
        modelTransform.rotation = Quaternion.LookRotation(new Vector3(dirToPlayer.x, 0f, dirToPlayer.z));
    }

    void OnTriggerEnter(Collider other) {
        if (other.transform.parent != null) {
            if (other.transform.parent.tag == "Player") {
                targetTransform = other.transform.parent;
                animator.SetBool("hasTarget", true);
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.transform.parent != null) {
            if (other.transform.parent.tag == "Player" && Vector3.Distance(other.transform.position, transform.position) >= spottingDistance) {
                targetTransform = null;
                animator.SetBool("hasTarget", false);
            }
        }
    }
}

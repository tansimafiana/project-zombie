using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavmesh : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;

    public float moveSpeed = 2.5f;
    private float _currSpeed;
    public float CurrSpeed {
        get { return _currSpeed; }
        set {
            _currSpeed = value;
            navMeshAgent.speed = value;
        }
    }

    Transform targetTransform;
    [SerializeField] private Transform modelTransform;

    [SerializeField] private EnemyHealthUI enemyHealthUI;
    public GameObject stunFX;

    private Animator animator;

    public Vector2 followOffset;
    [SerializeField] float maxDistanceLimit = 100f;


    void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Start() {
        animator = GetComponent<Animator>();

        CurrSpeed = moveSpeed;
        stunFX.SetActive(false);

        
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);

        followOffset = new Vector2(cos * 2f, sin * 2f);

        targetTransform = DirectorZombieSpawner.Instance.playerTransform;
    }

    void Update() {
        if (Vector3.Distance(transform.position, targetTransform.position) <= maxDistanceLimit) {
            Follow();
        }
    }

    void Follow() {
        navMeshAgent.destination = targetTransform.position + new Vector3(followOffset.x, 0f, followOffset.y);
    }

    public IEnumerator Stun(float speedMult, float stunTime) {
        CurrSpeed *= speedMult;
        stunFX.SetActive(true);
        yield return new WaitForSeconds(stunTime);
        CurrSpeed = moveSpeed;
        stunFX.SetActive(false);
    }
}

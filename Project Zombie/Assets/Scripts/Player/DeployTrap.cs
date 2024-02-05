using UnityEngine;

public class DeployTrap : MonoBehaviour
{
    [SerializeField] private TrapSO trap;
    [SerializeField] private Transform p_modelTransform;
    private PlayerMovement pl_movement;

    void Start() {
        pl_movement = GetComponent<PlayerMovement>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Q) && pl_movement._isGrounded) {
            Deploy();
        }
    }

    public void Deploy() {
        Transform _trap = Instantiate(trap.prefab).transform;
        _trap.position = p_modelTransform.position;
        _trap.localEulerAngles = p_modelTransform.localEulerAngles;
        _trap.localScale = trap.size;

        BoxCollider col = _trap.gameObject.AddComponent<BoxCollider>();
        col.isTrigger = true;

        _trap.gameObject.AddComponent<TrapBehaviour>().Init(GetComponent<PlayerStats>(), trap);
    }
}

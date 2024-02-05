using UnityEngine;

public class IdleRotate : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Vector3 direction;

    void Start() {
        if (direction == Vector3.zero) 
            direction = Vector3.up;
    }

    void LateUpdate()  { transform.Rotate(direction * rotationSpeed * Time.unscaledDeltaTime, Space.World); }
}

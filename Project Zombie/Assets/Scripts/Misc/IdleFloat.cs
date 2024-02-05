using UnityEngine;

public class IdleFloat : MonoBehaviour
{
    public float speed = 1;
    public float amplitude = 1;
    public bool hasOrigin = true;
    public Vector3 origin;

    void Start() {
        origin = (hasOrigin) ? origin : transform.position;
    }

    void LateUpdate() {
        transform.localPosition = origin + amplitude * Mathf.Sin(speed * Time.time) * Vector3.up;
    }
}

using UnityEngine;

public class PetFollow : MonoBehaviour
{
    public Transform petAnchor;
    public float speed = 1f;
    public float rotSpeed = 5f;

    public bool followPlayer = true;

    void LateUpdate() {
        if (followPlayer) {
            Vector3 dirToFace;// = petAnchor.position - transform.position;

            if (Vector3.Distance(transform.position, petAnchor.position) > 0.15f) {
                dirToFace = petAnchor.position - transform.position;
            } else {
                dirToFace = petAnchor.forward;
            }
            Quaternion targetRotation = Quaternion.LookRotation(dirToFace);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotSpeed * Time.unscaledDeltaTime);

            //Vector3 dir = (petAnchor.position - transform.position).normalized;
            //transform.Translate(speed * Time.unscaledDeltaTime * dir);

            transform.position = Vector3.MoveTowards(transform.position, petAnchor.position, speed * Time.unscaledDeltaTime);
        }
    }
}

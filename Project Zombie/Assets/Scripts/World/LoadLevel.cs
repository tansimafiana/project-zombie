using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    [SerializeField] private string sceneName;

    void Start() {
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    void OnTriggerEnter(Collider other) {
        if (other.transform.tag == "Player") {
            SceneManager.LoadScene(sceneName);
        }
    }
}

using UnityEngine;

public class DontDestoyOnLoad : MonoBehaviour
{
    void Start() {
        Object.DontDestroyOnLoad(gameObject);
    }
}

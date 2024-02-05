using System.Collections;
using UnityEngine;

public class AmmoSpawner : MonoBehaviour
{
    [SerializeField] GameObject ammoCratePrefab;

    [SerializeField] float timeToSpawn = 20f;

    void Start() {
        ammoCratePrefab = Instantiate(ammoCratePrefab, transform.position, Quaternion.identity);
        ammoCratePrefab.GetComponent<AmmoCrateBehaviour>().parent = this;
    }

    IEnumerator SpawnProtocol() {
        yield return new WaitForSeconds(timeToSpawn);
        ammoCratePrefab.SetActive(true);
    }

    public void CountdownSpawnCrate() {
        StartCoroutine(SpawnProtocol());
    }
}

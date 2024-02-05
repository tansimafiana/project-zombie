using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ZombieSpawnerBehaviour : NetworkBehaviour
{
    [SerializeField] private int[][] SpawnIDInfo;


    // We read the wave data from a JSON file using a an array for each wave, and the data in the array is strings.
    // Each string follows the format "X-X-X-X" where 'X' is a Zombie ID, '-' is the separator, and the length of the string determines the amount spawned
    #region JSon Reader Section
    public TextAsset textJSON;

    [System.Serializable]
    public class Wave {
        public string IDs;
    }

    [System.Serializable]
    public class WaveList {
        public Wave[] waves;
    }

    public WaveList waveList = new WaveList();
    #endregion


    // The public function that gets called by the Director when a new wave is ready. This will handle the spawning
    public void SpawnZombies(GameObject zombiePrefab, EnemySO[] zombieTypes, int wave) {
        StartCoroutine(SpawnAndCooldown(zombiePrefab, zombieTypes, wave));
    }

    // This makes sure that the cooldown is utilized
    IEnumerator SpawnAndCooldown(GameObject zombiePrefab, EnemySO[] types, int wave) {
        // Quick check for seeing if our spawner is empty...
        if (waveList.waves.Length < wave)  { yield break; }
        DirectorZombieSpawner.Instance.spawnersActive = true;

        // Reading JSON data...
        string data = waveList.waves[wave - 1].IDs;
        string[] split = (data.Contains('-')) ? data.Split('-') : new []{data};
        //int[] IDs = Array.ConvertAll(split, int.Parse);     // Converts the string array IDs to integer array IDs

        // Instead of adding +1 to the Director's zombie counter each time we spawn a new zombie, we'll directly add it now based on the ID's lenght.
        // This method is way better because it has less calls to the counter variables (faster), and it eliminates any bugs that could appear if the
        //  only spawned zombie is killed straight away.                NOTE:  An ID of 0 means BLANK (no spawn).
        //DirectorZombieSpawner.Instance.ActiveZombiesInScene += IDs.Length;

        int ID = 0;
        int amount = 1;
        float spawnCooldown = 2f;

        foreach (string spawnInfo in split) {
            // Now that we have our IDs, we split them up.
            // First number is the ID, number after the ':' is the amount, number after '/' is the Spawn Cooldown

            if (spawnInfo.Contains(':')) {
                string[] infoSplit = spawnInfo.Split(':');
                ID = int.Parse(infoSplit[0]);

                if (ID == 0)  continue;

                if (spawnInfo.Contains('/')) {      // If it has a timestamp, we do a separate operation
                    string[] amountTimeSplit = infoSplit[1].Split('/');
                    amount = int.Parse(amountTimeSplit[0]);
                    spawnCooldown = float.Parse(amountTimeSplit[1]);
                } else {
                    amount = int.Parse(infoSplit[1]);
                    spawnCooldown = 0.1f;
                }
            } else {
                if (spawnInfo.Contains('/')) {
                    string[] IDTimeSplit = spawnInfo.Split('/');
                    ID = int.Parse(IDTimeSplit[0]);
                    spawnCooldown = float.Parse(IDTimeSplit[1]);
                } else {
                    ID = int.Parse(spawnInfo);
                }

                if (ID == 0)  continue;
            }

            Debug.Log("ID: " + ID + "\tAmount: " + amount + "\tCooldown: " + spawnCooldown);
            DirectorZombieSpawner.Instance.ActiveZombiesInScene += amount;


            /*GameObject newZomb = Instantiate(zombiePrefab, transform.position, Quaternion.identity);
            UpdateZombie(newZomb, types[ID - 1]);

            for (int i = 0; i < amount - 1; i++) {
                UpdateStats(Instantiate(newZomb, transform.position, Quaternion.identity), types[ID - 1]);
                yield return new WaitForSeconds(spawnCooldown);
            }*/

            for (int i = 0; i < amount; i++) {
                GameObject newZomb = Instantiate(zombiePrefab, transform.position, Quaternion.identity);
                UpdateZombie(newZomb, types[ID - 1]);
                yield return new WaitForSeconds(spawnCooldown);
            }

            yield return new WaitForSeconds(spawnCooldown);
        }
    }
    

    // We take the newly instantiated Zombie, take the type, and update the zombie based on its type    (Which doesn't work lol)
    void UpdateZombie(GameObject zombie, EnemySO type) {
        //Debug.Log("<color=green>Material name: " + zombie.GetComponentInChildren<MeshRenderer>().material.ToString() + "</color>");
        zombie.GetComponentInChildren<SkinnedMeshRenderer>().material.SetTexture("_MainTex", type.texture);

        zombie.GetComponent<EnemyStats>().UpdateStats(0.1f, type.health, type.speed, type.damage);
        zombie.transform.localScale = type.scale;
        zombie.name = type.name;
    }

    void UpdateStats(GameObject zombie, EnemySO type) {
        // I didn't wanna do it this way, but it leaves no choice
        zombie.GetComponent<EnemyStats>().UpdateStats(0.1f, type.health, type.speed, type.damage);
        //zombie.transform.localScale = type.scale;
        zombie.name = type.name;
    }
}

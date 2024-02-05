using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public Transform parent;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    #region Singleton
    public static ObjectPooler Instance;

    void Awake() {
        Instance = this;
        Debug.Log("<color=gray>Object Pooling Singleton Instance!</color>");
    }
    #endregion

    void Start() {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
    
        foreach (Pool pool in pools) {
            Queue<GameObject> objPool = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++) {
                GameObject obj = Instantiate(pool.prefab, (pool.parent == null) ? transform : pool.parent);

                if (obj.TryGetComponent<ParticleSystem>(out ParticleSystem system)) {   // Implementing Particle resetting...
                    obj.AddComponent<AwakeParticleReset>().particleSystem = system;
                }

                obj.SetActive(false);                                                                                         
                objPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 pos, Quaternion rot) {
        if (!poolDictionary.ContainsKey(tag)) {
            Debug.LogWarning("[ObjectPooler] Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        GameObject objToSpawn = poolDictionary[tag].Dequeue();

        objToSpawn.SetActive(true);
        objToSpawn.transform.position = pos;
        objToSpawn.transform.rotation = rot;

        IPooledObject pooledObj = objToSpawn.GetComponent<IPooledObject>();
        
        if (pooledObj != null) {
            pooledObj.OnObjectSpawn();
        }

        poolDictionary[tag].Enqueue(objToSpawn);

        return objToSpawn;
    }
}

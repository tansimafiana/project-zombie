using UnityEngine;

[CreateAssetMenu(fileName = "New Zombie Type", menuName = "Zombie Type")]
public class EnemySO : ScriptableObject
{
    public string zombieName;
    public Texture texture;
    public int spawnWeight;
    public float health;
    public float speed;
    public float damage;
    public Vector3 scale = Vector3.one;
}

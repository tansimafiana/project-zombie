using UnityEngine;

[CreateAssetMenu(fileName = "New Throwable", menuName = "Throwable")]
public class ThrowSO : ScriptableObject
{
    public string _name;
    public float damage;
    public float effectRadius;
    [Range(0f, 1f)]public float stunPercentSpeed;
    public float activationTime;
    public float stunTime;
    [Space]
    public float minPushbackForce;
    public float maxPushbackForce;

    [Space]
    public GameObject prefab;
    public Vector3 size;
    public GameObject particles;
}

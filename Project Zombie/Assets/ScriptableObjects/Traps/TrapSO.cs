using UnityEngine;

[CreateAssetMenu(fileName = "New Trap", menuName = "Trap")]
public class TrapSO : ScriptableObject
{
    public string _name;
    public float damage;
    public float stunPercentSpeed;
    public float stunTime;

    [Space]
    public GameObject prefab;
    public Vector3 size;
}

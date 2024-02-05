using UnityEngine;

[CreateAssetMenu(fileName = "New Hat", menuName = "Hat")]
public class HatSO : ScriptableObject
{
    public string _name;
    public GameObject prefab;
    public int levelLock;

    [Space]
    [Header("Selection Panel Local Attributes")]
    public Vector3 shopPos;
    public Vector3 shopRot;
    public Vector3 shopScale;

    [Space]
    [Header("Player Head Local Attributes")]
    public Vector3 headPos;
    public Vector3 headRot;
    public Vector3 headScale;

    void OnEnable() {
        if (prefab == null)  { prefab = new GameObject(); }
    }
}

using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon")]
public class GunSO : ScriptableObject
{
    public string weaponName;
    public int cost;
    public GameObject prefab;
    public GameObject particles;
    [Space]
    public Vector3 handheldPos;
    public Vector3 handheldRot;
    public Vector3 handheldScl;
    public Vector3 fireLocalPos;
    public Vector3 fireLocalRot;
    [Space]
    public bool isMelee;
    public bool isShotgun;
    public bool isSniper;
    [Space]
    public float damage;
    public int shotgunPellets;
    public float maxSpread;
    public float range;
    public float timeBtwShots;
    public bool isAuto;
    public float zoomAmount;
    [Space]
    public int clipSize;
    public int ammoAmount;
    [Space]
    public AudioClip s_gunshot;
    [Range(0f, 1f)] public float s_volume;
}

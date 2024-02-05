using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skin", menuName = "Skin")]
public class SkinSO : ScriptableObject
{
    public string _name;
    public Texture texture;
    public int levelLock;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Tower")]
public class TowerSO : ScriptableObject
{
    public string towerName;
    public string description;
    public GameObject visual;
    public Sprite icon;
    public string abilityId;
}

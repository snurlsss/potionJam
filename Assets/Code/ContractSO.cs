using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Data/Contract")]
public class ContractSO : SerializedScriptableObject
{
    public string companyName = string.Empty;
    public string description = string.Empty;
    public string abilityId = string.Empty;

    [Space]
    public Contract visual;
    public Sprite logo;
    public Color backColor = Color.white;
    public Color frontColor = Color.white;
}

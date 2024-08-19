using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Flavors")]
public class FlavorMapping : SerializedScriptableObject
{
    [SerializeField] Dictionary<Flavor, FlavorData> entries = new();

    public bool TryGet(Flavor key, out FlavorData flavor)
    {
        return entries.TryGetValue(key, out flavor);
    }
}

[Serializable]
public class FlavorData
{
    public string name;
    public Color color;
}

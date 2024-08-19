using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Bottles")]
public class BottleMapping : SerializedScriptableObject
{
    [SerializeField] Dictionary<Bottle, BottleData> entries = new();

    public bool TryGet(Bottle key, out BottleData flavor)
    {
        return entries.TryGetValue(key, out flavor);
    }
}

[Serializable]
public class BottleData
{
    public string name;
    public Sprite bottleSprite;
    public Sprite liquidSprite;
}

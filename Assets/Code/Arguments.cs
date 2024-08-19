using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arguments
{
    Dictionary<string, object> values = new();

    public void Set(string key, object value)
    {
        values[key] = value;
    }

    public bool TryGet(string key, out object value)
    {
        return values.TryGetValue(key, out value);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoKeyDictionary 
{
    public Dictionary<Tuple<Vector2, Vector2>, bool> keyValues = new Dictionary<Tuple<Vector2, Vector2>, bool>();

    public bool HaveElement(Vector2 key1, Vector2 key2)
    {
        bool result = false;
        //Normal key values
        Tuple<Vector2, Vector2> normalResult = new Tuple<Vector2, Vector2>(key1, key2);
        result = keyValues.ContainsKey(normalResult);
        if (result)
            return true;

        //Mirror key values
        Tuple<Vector2, Vector2> mirrorResult = new Tuple<Vector2, Vector2>(key2, key1);
        result = keyValues.ContainsKey(mirrorResult);
        if (result)
            return true;

        return false;
    }

    public void AddElement(Vector2 key1, Vector2 key2)
    {
        Tuple<Vector2, Vector2> normalResult = new Tuple<Vector2, Vector2>(key1, key2);
        if (!keyValues.ContainsKey(normalResult))
            keyValues.Add(normalResult, true);
        Tuple<Vector2, Vector2> mirrorResult = new Tuple<Vector2, Vector2>(key2, key1);
        if (!keyValues.ContainsKey(mirrorResult))
            keyValues.Add(mirrorResult, true);
    }
}

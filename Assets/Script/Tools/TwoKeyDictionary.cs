using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoKeyDictionary<T>
{
    public Dictionary<Tuple<T, T>, bool> keyValues = new Dictionary<Tuple<T, T>, bool>();

    public bool HaveElement(T key1, T key2)
    {
        // Check normal key values
        Tuple<T, T> normalResult = new Tuple<T, T>(key1, key2);
        if (keyValues.ContainsKey(normalResult))
            return true;

        // Check mirror key values
        Tuple<T, T> mirrorResult = new Tuple<T, T>(key2, key1);
        if (keyValues.ContainsKey(mirrorResult))
            return true;

        return false;
    }

    public void AddElement(T key1, T key2)
    {
        // Add normal key values
        Tuple<T, T> normalResult = new Tuple<T, T>(key1, key2);
        if (!keyValues.ContainsKey(normalResult))
            keyValues.Add(normalResult, true);

        // Add mirror key values
        Tuple<T, T> mirrorResult = new Tuple<T, T>(key2, key1);
        if (!keyValues.ContainsKey(mirrorResult))
            keyValues.Add(mirrorResult, true);
    }
}

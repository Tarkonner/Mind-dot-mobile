using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoKeyDictionary 
{
    public Dictionary<Tuple<Dot, Dot>, bool> keyValues = new Dictionary<Tuple<Dot, Dot>, bool>();

    public bool HaveElement(Dot key1, Dot key2)
    {
        bool result = false;
        //Normal key values
        Tuple<Dot, Dot> normalResult = new Tuple<Dot, Dot>(key1, key2);
        result = keyValues.ContainsKey(normalResult);
        if (result)
            return true;

        //Mirror key values
        Tuple<Dot, Dot> mirrorResult = new Tuple<Dot, Dot>(key2, key1);
        result = keyValues.ContainsKey(mirrorResult);
        if (result)
            return true;

        return false;
    }

    public void AddElement(Dot key1, Dot key2)
    {
        Tuple<Dot, Dot> normalResult = new Tuple<Dot, Dot>(key1, key2);
        if (!keyValues.ContainsKey(normalResult))
            keyValues.Add(normalResult, true);
        Tuple<Dot, Dot> mirrorResult = new Tuple<Dot, Dot>(key2, key1);
        if (!keyValues.ContainsKey(mirrorResult))
            keyValues.Add(mirrorResult, true);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConnectionData<T>
{
    public T key1;
    public T key2;
}

[System.Serializable]
public class TwoKeyDictionary<T>
{
    [SerializeField] private List<ConnectionData<T>> connections = new List<ConnectionData<T>>();

    public void AddElement(T key1, T key2)
    {
        // Check if the connection already exists
        if (!HaveElement(key1, key2))
        {
            // Add both normal and mirrored connections
            connections.Add(new ConnectionData<T> { key1 = key1, key2 = key2 });
            connections.Add(new ConnectionData<T> { key1 = key2, key2 = key1 });
        }
    }

    public bool HaveElement(T key1, T key2)
    {
        foreach (var connection in connections)
        {
            if ((connection.key1.Equals(key1) && connection.key2.Equals(key2)) ||
                (connection.key1.Equals(key2) && connection.key2.Equals(key1)))
            {
                return true;
            }
        }
        return false;
    }

    public List<ConnectionData<T>> GetConnections()
    {
        return connections;
    }
}
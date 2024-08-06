using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class DotPool : PoolerBase<Dot>
{
    public static DotPool instance;

    [SerializeField] private Dot dotPrefab;
    [SerializeField] int defultSpawnedDots = 50;
    [SerializeField] int maxSpawnedDots = 200;

    private void Awake()
    {
        instance = this;

        InitPool(dotPrefab, defultSpawnedDots, maxSpawnedDots); // Initialize the pool

        for (int i = 0; i < defultSpawnedDots; i++)
        {
            var spawn = CreateSetup();
            Release(spawn);
        }
    }


    public GameObject GetDot()
    {
        return pool.Get().gameObject;
    }
}

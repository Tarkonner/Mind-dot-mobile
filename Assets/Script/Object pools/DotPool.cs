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
            CreateSetup();
        }
    }


    public GameObject GetDot(DotType whatKindOfDot, Piece parentPiece = null)
    {
        Dot target = pool.Get();
        target.Setup(whatKindOfDot, parentPiece);
        return target.gameObject;
    }

    protected override void ReleaseSetup(Dot obj)
    {
        obj.transform.parent = null;
        obj.transform.position = Vector3.zero;
    }
}

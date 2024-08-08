using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellPool : PoolerBase<Cell>
{
    public static CellPool instance;

    [SerializeField] private Cell cellPrefab;
    [SerializeField] int defultSpawnedDots = 7 * 7;
    [SerializeField] int maxSpawnedDots = 7 * 7;

    private void Awake()
    {
        instance = this;

        InitPool(cellPrefab, defultSpawnedDots, maxSpawnedDots); // Initialize the pool

        for (int i = 0; i < defultSpawnedDots; i++)
            CreateSetup();
    }


    protected override void ReleaseSetup(Cell obj)
    {
        base.ReleaseSetup(obj);
        obj.transform.parent = null;
        obj.occupying = null;
        obj.gridPos = Vector2Int.zero;
    }
}

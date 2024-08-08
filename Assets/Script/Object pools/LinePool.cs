using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LinePool : PoolerBase<UILine>
{
    public static LinePool instance;

    [SerializeField] UILine linePrefab;
    [SerializeField] int defultSpawnedDots = 10;
    [SerializeField] int maxSpawnedDots = 6 * 6;

    private void Awake()
    {
        instance = this;

        InitPool(linePrefab, defultSpawnedDots, maxSpawnedDots); // Initialize the pool

        for (int i = 0; i < defultSpawnedDots; i++)
            CreateSetup();
    }

    public UILine GetLine(RectTransform startPoint, RectTransform endPoint, float lineWidth, bool rotatebul = true)
    {
        UILine targetLine = Get();
        targetLine.Initialzie(startPoint, endPoint, lineWidth, rotatebul);
        return targetLine;
    }
}

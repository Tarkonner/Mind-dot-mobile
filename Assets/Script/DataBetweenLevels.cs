using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBetweenLevels : MonoBehaviour
{
    public static DataBetweenLevels Instance;

    public int targetLevel = 0;

    public LevelsBank currentLevelChunk;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public LevelSO GetCurretLevel()
    {
        return currentLevelChunk.levels[targetLevel];
    }
}

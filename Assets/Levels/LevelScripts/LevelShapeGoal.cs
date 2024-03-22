using SharedData;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelShapeGoal
{
    public Vector2[] goalSpecifications;
    public DotType[] goalDots;
    public Vector2Int gridPosRef;
    public Vector2Int goalSize;

    public LevelShapeGoal(GridData goal)
    {
        goalSpecifications = new Vector2[goal.dotDictionary.Keys.Count];
        goalDots = new DotType[goal.dotDictionary.Values.Count];
        int i = 0;
        foreach (var dotPos in goal.dotDictionary.Keys)
        {
            goalSpecifications[i] = dotPos;
            i++;
        }
        i = 0;
        foreach (var dotType in goal.dotDictionary.Values)
        {
            goalDots[i] = dotType.dotType;
            i++;
        }

        gridPosRef = goal.gridPosRef;

        goalSize = goal.gridSize;
    }
    public LevelShapeGoal(Vector2[] goalSpecifications, DotType[] goalDots)
    {
        this.goalSpecifications = goalSpecifications;
        this.goalDots = goalDots;
    }
}
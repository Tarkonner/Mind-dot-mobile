using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelShapeGoal
{
    public Vector2[] goalSpecifications;
    public DotType[] goalDots;
    public Vector2Int gridPosRef;
    public LevelShapeGoal(ShapeGoal goal)
    {
        goalSpecifications = new Vector2[goalSpecifications.Length];
        goalDots = new DotType[goalSpecifications.Length];
        for (int i = 0; i < goal.goalSpecifications.Length; i++)
        {
            goalSpecifications[i] = goal.goalSpecifications[i].gridPos;
            goalDots[i] = goal.goalSpecifications[i].dotType;
        }
    }
    public LevelShapeGoal(List<Vector2Int> goalSpecifications, List<DotElement> goalDots, Vector2Int gridposref)
    {
        this.goalSpecifications = new Vector2[goalSpecifications.Count];
        int i = 0;
        foreach (var pos in goalSpecifications) 
        {
            this.goalSpecifications[i]= pos;
            i++;            
        }
        i = 0;
        this.goalDots = new DotType[goalDots.Count];
        foreach (var dot in goalDots)
        {
            this.goalDots[i] = goalDots[i].dotType;
            i++;
        }
        gridPosRef = gridposref;
    }
    public LevelShapeGoal(ShapeGoalElement goal)
    {
        goalSpecifications = new Vector2[goal.dotDictionary.Keys.Count];
        goalDots = new DotType[goal.dotDictionary.Values.Count];
        int i = 0;
        foreach(var dotPos in goal.dotDictionary.Keys)
        {
            goalSpecifications[i] = dotPos;
            i++;
        }
        i= 0;
        foreach( var dotType in goal.dotDictionary.Values)
        {
            goalDots[i] = dotType.dotType; 
            i++;
        }

        gridPosRef = goal.gridPosRef;
    }
    public LevelShapeGoal(Vector2[] goalSpecifications, DotType[] goalDots)
    {
        this.goalSpecifications = goalSpecifications;
        this.goalDots = goalDots;
    }
}
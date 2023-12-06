using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelShapeGoal
{
    public Vector2[] goalSpecifications;
    public DotType[] goalDots;

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
    public LevelShapeGoal(List<Vector2Int> goalSpecifications, List<DotElement> goalDots)
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
    }
    public LevelShapeGoal(Vector2[] goalSpecifications, DotType[] goalDots)
    {
        this.goalSpecifications = goalSpecifications;
        this.goalDots = goalDots;
    }
}
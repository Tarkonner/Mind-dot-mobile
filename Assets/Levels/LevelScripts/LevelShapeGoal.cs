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
    public LevelShapeGoal(Vector2[] goalSpecifications, DotType[] goalDots)
    {
        this.goalSpecifications = goalSpecifications;
        this.goalDots = goalDots;
    }
}
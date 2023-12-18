using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalMaker : MonoBehaviour
{
    [SerializeField] GameObject shapeGoalPrefab;
    [SerializeField] Transform holder;
   

    List<ShapeGoal> goals = new List<ShapeGoal>();

    public void RemoveAllGoals()
    {

    }

    public void MakeGoals(LevelShapeGoal[] levelGoals)
    {
        for (int i = 0; i < levelGoals.Length; i++)
        {
            GameObject spawnedGoal = Instantiate(shapeGoalPrefab, holder);
            ShapeGoal shapeGoal = spawnedGoal.GetComponent<ShapeGoal>();
            shapeGoal.LoadGoal(levelGoals[i]);
        }
    }
}

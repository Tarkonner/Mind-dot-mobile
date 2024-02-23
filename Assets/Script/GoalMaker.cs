using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalMaker : MonoBehaviour
{
    [SerializeField] GameObject shapeGoalPrefab;
    [SerializeField] Transform holder;   


    public void MakeGoals(LevelShapeGoal[] levelGoals)
    {
        if(holder.transform.childCount > 0)
        {
            for (int i = holder.transform.childCount - 1; i >= 0; i--)
                Destroy(holder.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < levelGoals.Length; i++)
        {
            GameObject spawnedGoal = Instantiate(shapeGoalPrefab, holder);
            ShapeGoal shapeGoal = spawnedGoal.GetComponent<ShapeGoal>();
            shapeGoal.LoadGoal(levelGoals[i]);
        }
    }
}

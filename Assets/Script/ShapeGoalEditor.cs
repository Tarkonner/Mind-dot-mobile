using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShapeGoal))]
public class ShapeGoalEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ShapeGoal shapeGoal = (ShapeGoal)target;

        DrawDefaultInspector();

        if(GUILayout.Button("Add GoalDot"))
        {
            Array.Resize(ref shapeGoal.goalSpecifications, shapeGoal.goalSpecifications.Length+1);
            shapeGoal.goalSpecifications[shapeGoal.goalSpecifications.Length - 1] = new GoalDot();
        }
    }
}

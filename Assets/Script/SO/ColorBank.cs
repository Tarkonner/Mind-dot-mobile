using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Colorbank", menuName = "ColorBank")]
public class ColorBank : ScriptableObject
{
    [Header("Dots color")]
    public Color redColor;
    public Color yellowColor;
    public Color blueColor;

    [Header("Placegoal outline color")]
    public Color backgroundNull;
    public Color backgroundRedColor;
    public Color backgroundYellowColor;
    public Color backgroundBlueColor;

    [Header("Goal color")]
    public Color completeGoal;
    public Color uncompleteGoal;
}

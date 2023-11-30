using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSO : ScriptableObject
{
    private string version;
    public string levelTitle;

    public LevelBoard levelGrid;

    public LevelPiece[] levelPieces;

    public LevelShapeGoal[] levelGoals;

    public LevelPlaceGoal[] levelPlaceGoals;
}

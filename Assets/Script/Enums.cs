using System;

[Serializable]
public enum DotType
{
    Null,
    Blue,
    Yellow,
    Red
}

[Serializable]
public enum CellColorState
{
    partGoal,
    partPiece,
    partGoalAndPiece,
    choosenPiece,
    choosenGoal,
    turnedOff,
    normal
};
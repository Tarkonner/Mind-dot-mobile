using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakePlaceGoalState : EditorState
{
    public void Execute(CellElement targetCell, int buttonIndex, LevelEditor levelEditor)
    {
        if (targetCell.cellData.turnedOff)
            return;

        if(buttonIndex == 1) //Right click
        {
            if (!levelEditor.placeGoalCells.Contains(targetCell))
                return;

            levelEditor.placeGoalCells.Remove(targetCell);
            targetCell.RemovePlacementGoal();
        }
        else
        {
            if (!levelEditor.placeGoalCells.Contains(targetCell))
            {
                levelEditor.placeGoalCells.Add(targetCell);
                targetCell.AddPlacementGoal(DotType.Null);
            }
            else
            {
                targetCell.placeGoal.ChangeColor();
            }
        }
    }
}

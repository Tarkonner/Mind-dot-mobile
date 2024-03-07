using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceDotState : EditorState
{

    public override void Execute()
    {
        Debug.LogError("Called place dot without index or cell");
    }

    public void Execute(DotType dotType, int buttonIndex, CellElement cell)
    {
        if (cell.cellData.turnedOff)
            return;

        if (buttonIndex == 1) //Right click
        {
            cell.RemoveDot();
            return;
        }

        if (cell.cellData.partOfPiece || cell.partOfShapeGoals.Count > 0)
            return;


        if (buttonIndex == 0) //Left click
        {
            if (cell.cellData.holding != null)
                cell.ChangeDotColor();
            else
                cell.SetDot(new DotElement(dotType));
        }
    }
}

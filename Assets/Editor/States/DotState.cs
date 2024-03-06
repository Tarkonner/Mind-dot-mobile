using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DotState : EditorStats
{
    public override void Execute(VisualElement target)
    {
        if(target == null || target !is CellElement)
        {
            Debug.LogError("Not a cell or was empyu");
            return;
        }    


    }

    private void PlaceDot(CellElement targetCell, int buttonIndex)
    {
        if (targetCell.cellData.turnedOff)
            return;

        if (buttonIndex == 1) //Right click
        {
            targetCell.RemoveDot();
            return;
        }

        if (targetCell.cellData.partOfPiece || targetCell.partOfShapeGoals.Count > 0)
            return;


        if (buttonIndex == 0) //Left click
        {
            if (targetCell.cellData.holding != null)
                targetCell.ChangeDotColor();
            else
            {
                DotElement targetDot = null;
                switch (0)
                {
                    case 0:
                        targetDot = new DotElement(DotType.Red);
                        break;
                    case 1:
                        targetDot = new DotElement(DotType.Blue);
                        break;
                    case 2:
                        targetDot = new DotElement(DotType.Yellow);
                        break;
                }

                targetCell.SetDot(targetDot);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CollectCells : EditorState
{
    protected List<CellElement> cells;

    public override void Enter()
    {
        cells = new List<CellElement>();
    }

    public virtual void AddCell(CellElement targetCell, CellColorState targetState)
    {
        if (targetCell.cellData.turnedOff)
            return;

        if (!cells.Contains(targetCell))
        {
            cells.Add(targetCell);
            targetCell.ChangeCellColor(targetState);
        }
        else
        {
            cells.Remove(targetCell);
            targetCell.SetDefultColor();
        }
    }

    protected void RemoveEmptyCells()
    {
        for (int i = cells.Count - 1; i >= 0; i--)
        {
            if (cells[i].cellData.holding == null)
            {
                cells.RemoveAt(i);
                cells[i].SetDefultColor();
            }
        }
    }
}

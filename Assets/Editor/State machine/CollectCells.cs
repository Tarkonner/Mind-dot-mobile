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

    public void PremakeCells(List<CellElement> inputCells)
    {
        cells = inputCells;
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
            targetCell.SetDefultColor();
            cells.Remove(targetCell);
        }
    }

    protected void RemoveEmptyCells()
    {
        for (int i = cells.Count - 1; i >= 0; i--)
        {
            if (cells[i].cellData.holding == null)
            {
                cells[i].SetDefultColor();
                cells.RemoveAt(i);
            }
        }
    }

    public override void Exit()
    {
        for (int i = cells.Count - 1; i >= 0; i--)
        {
            if (cells[i].myColorState != CellColorState.partPiece)
                cells[i].SetDefultColor();
        }

    }
}

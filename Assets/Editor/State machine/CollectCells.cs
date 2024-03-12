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

    public void AddCell(CellElement targetCell)
    {
        if(cells.Contains(targetCell))
            cells.Remove(targetCell);
        else
            cells.Add(targetCell);
    }
}

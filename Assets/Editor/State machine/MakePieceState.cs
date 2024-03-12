using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MakePieceState : CollectCells
{
    public override void AddCell(CellElement targetCell, CellColorState targetState)
    {
        if (targetCell.cellData.partOfPiece)
            return;

        base.AddCell(targetCell, targetState);
    }

    public void Execute(VisualElement holder, VisualTreeAsset spawnHolder)
    {
        if (cells.Count == 0)
        {
            Debug.Log("No cells selected");
            return;
        }

        holder.Add(spawnHolder.Instantiate());
        
        cells.Clear();
    }
}

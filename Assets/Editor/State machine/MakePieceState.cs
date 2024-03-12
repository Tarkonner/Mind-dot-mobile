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

    public void Execute(VisualElement holder, VisualTreeAsset spawnHolder, LevelEditor levelEditor)
    {    
        //Clean data
        RemoveEmptyCells();
        if (cells.Count == 0)
        {
            Debug.Log("No cells selected");
            return;
        }

        //Set Color for cells
        for (int i = 0; i < cells.Count; i++)
            cells[i].ChangeCellColor(CellColorState.partPiece);

        //Data
        PieceElement pieceElement = new PieceElement(levelEditor);

        levelEditor.piecesData.Add(pieceElement);

        //Editor
        VisualElement pieceHolder = spawnHolder.Instantiate();
        pieceHolder.Q<Button>("Delete").clickable.clicked += () => holder.Remove(pieceHolder);

        holder.Add(pieceHolder);

        cells.Clear();
    }
}

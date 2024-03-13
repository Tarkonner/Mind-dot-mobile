using SharedData;
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
        {
            cells[i].ChangeCellColor(CellColorState.partPiece);
            cells[i].cellData.partOfPiece = true;
        }

        //Data
        PieceElement pieceElement = new PieceElement();

        levelEditor.piecesData.Add(pieceElement);

        //Editor
        VisualElement pieceHolder = spawnHolder.Instantiate();
        pieceHolder.Q<Button>("Delete").clickable.clicked += () => { holder.Remove(pieceHolder); levelEditor.piecesData.Remove(pieceElement); };

        //Grid
        pieceHolder.Q<VisualElement>("Grid").Add(GridMaker.MakeGridElement(cells, pieceElement));
        SliderInt rotationSlider = pieceHolder.Q<SliderInt>("RotateValue");
        rotationSlider.RegisterValueChangedCallback(value => ((PieceData)pieceElement.gridData).startRotationIndex = value.newValue);
        pieceHolder.Q<Toggle>("RotatebulToggle").RegisterValueChangedCallback(value => ((PieceData)pieceElement.gridData).canRotate = value.newValue);

        holder.Add(pieceHolder);

        cells.Clear();
    }

    public void SaveData()
    {
        
    }
}

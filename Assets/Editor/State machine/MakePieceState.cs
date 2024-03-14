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
            cells[i].cellData.partOfPiece = true;

            if (cells[i].myColorState == CellColorState.partGoal)
                cells[i].ChangeCellColor(CellColorState.partGoalAndPiece);
            else
                cells[i].ChangeCellColor(CellColorState.partPiece);
        }

        //Data
        PieceElement pieceElement = new PieceElement();

        levelEditor.piecesData.Add(pieceElement);

        //Connect behavior
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

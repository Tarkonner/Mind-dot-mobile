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



        //Data
        PieceElement pieceElement = new PieceElement();
        levelEditor.piecesData.Add(pieceElement);

        //Set Color for cells
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].cellData.partOfPiece = true;

            cells[i].SetPiece(pieceElement);
        }


        //Grid
        VisualElement pieceHolder = spawnHolder.Instantiate();
        pieceHolder.Q<VisualElement>("Grid").Add(GridMaker.MakeGridElement(cells, pieceElement));
        
        //Gate
        if(!pieceElement.legalPiece)
            return;

        //Editor
        pieceHolder.Q<Button>("Delete").clickable.clicked += () => { holder.Remove(pieceHolder); levelEditor.RemovePiece(pieceElement); };
        //Slider
        SliderInt rotationSlider = pieceHolder.Q<SliderInt>("RotateValue");
        rotationSlider.RegisterValueChangedCallback(value => ((PieceData)pieceElement.gridData).startRotationIndex = value.newValue);
        //Toggle
        pieceHolder.Q<Toggle>("RotatebulToggle").RegisterValueChangedCallback(value => {
            ((PieceData)pieceElement.gridData).canRotate = value.newValue;
            if (!value.newValue)
            {
                rotationSlider.value = 0;
                rotationSlider.SetEnabled(false);
            }
            else
                rotationSlider.SetEnabled(true);
        });

        holder.Add(pieceHolder);

        cells.Clear();
    }

    public void Execute(VisualElement holder, VisualTreeAsset spawnHolder, LevelPiece savedPiece, LevelEditor levelEditor)
    {
        List<CellElement> targetCells = new List<CellElement>();
        //Collect Cells and set color
        for (int i = 0; i < savedPiece.dotPositions.Length; i++)
        {
            CellElement cell = levelEditor.cells[(int)((savedPiece.dotPositions[i].y + savedPiece.gridPosRef.y) * 7 + (savedPiece.dotPositions[i].x + savedPiece.gridPosRef.x))];
            cell.ChangeCellColor(CellColorState.partPiece);
            targetCells.Add(cell);
        }

        //Data
        PieceElement pieceElement = new PieceElement();

        levelEditor.piecesData.Add(pieceElement);

        //Connect behavior
        //Editor
        VisualElement pieceHolder = spawnHolder.Instantiate();
        pieceHolder.Q<Button>("Delete").clickable.clicked += () => 
        { 
            holder.Remove(pieceHolder); 
            levelEditor.piecesData.Remove(pieceElement);
            for (int i = 0; i < targetCells.Count; i++)
                targetCells[i].RemovePiece();
        };
        //Grid
        pieceHolder.Q<VisualElement>("Grid").Add(GridMaker.MakeGridElement(cells, pieceElement));
        //Slider
        SliderInt rotationSlider = pieceHolder.Q<SliderInt>("RotateValue");
        rotationSlider.value = savedPiece.startRotation;
        rotationSlider.RegisterValueChangedCallback(value => ((PieceData)pieceElement.gridData).startRotationIndex = value.newValue);
        if (!savedPiece.rotatable)
            rotationSlider.SetEnabled(false);
        //Toggle
        Toggle rotationToggle = pieceHolder.Q<Toggle>("RotatebulToggle");
        rotationToggle.value = savedPiece.rotatable;
        rotationToggle.RegisterValueChangedCallback(value => {
            ((PieceData)pieceElement.gridData).canRotate = value.newValue;
            if (!value.newValue)
            {
                rotationSlider.value = 0;
                rotationSlider.SetEnabled(false);
            }
            else
                rotationSlider.SetEnabled(true);
        });

        holder.Add(pieceHolder);

        cells.Clear();
    }
}

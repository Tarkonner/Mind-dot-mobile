using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MakeShapeGoalState : CollectCells
{
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
            if (cells[i].myColorState == CellColorState.partPiece)
                cells[i].ChangeCellColor(CellColorState.partGoalAndPiece);
            else
                cells[i].ChangeCellColor(CellColorState.partGoal);
        }

        //Setup data
        ShapeGoalElement shapeGoalElement = new ShapeGoalElement();
        levelEditor.shapeGoals.Add(shapeGoalElement);
        
        //Connect behavior
        VisualElement goalHolder = spawnHolder.Instantiate();
        goalHolder.Q<Button>("Delete").clickable.clicked += () => { holder.Remove(goalHolder); levelEditor.shapeGoals.Remove(shapeGoalElement); };

        //Grid
        goalHolder.Q<VisualElement>("Holder").Add(GridMaker.MakeGridElement(cells, shapeGoalElement));

        holder.Add(goalHolder);
        cells.Clear();
    }
}

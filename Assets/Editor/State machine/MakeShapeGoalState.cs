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


        //Setup data
        ShapeGoalElement shapeGoalElement = new ShapeGoalElement();
        levelEditor.shapeGoals.Add(shapeGoalElement);
        
        //Set Color for cells
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].SetGoal(shapeGoalElement);
        }

        //Connect behavior
        VisualElement goalHolder = spawnHolder.Instantiate();
        goalHolder.Q<Button>("Delete").clickable.clicked += () => { holder.Remove(goalHolder); levelEditor.RemoveGoal(shapeGoalElement); };
        shapeGoalElement.holder = goalHolder;

        //Grid
        goalHolder.Q<VisualElement>("Holder").Add(GridMaker.MakeGridElement(cells, shapeGoalElement));

        holder.Add(goalHolder);
        cells.Clear();
    }

    //Then loading
    public void Execute(VisualElement holder, VisualTreeAsset spawnHolder, LevelShapeGoal shapeGoal, LevelEditor levelEditor)
    {
        List<CellElement> cells = new List<CellElement>();

        //Set Color for cells
        for (int i = 0; i < shapeGoal.goalSpecifications.Length; i++)
        {
            CellElement target = levelEditor.interactiveGrid.cells[(int)((shapeGoal.goalSpecifications[i].y + shapeGoal.gridPosRef.y) * InteractiveGrid.gridSize + (shapeGoal.goalSpecifications[i].x + shapeGoal.gridPosRef.x))];

            target.ChangeCellColor(CellColorState.partGoal);
            cells.Add(target);
        }            

        //Setup data
        ShapeGoalElement shapeGoalElement = new ShapeGoalElement();
        levelEditor.shapeGoals.Add(shapeGoalElement);

        //Connect behavior
        VisualElement goalHolder = spawnHolder.Instantiate();
        goalHolder.Q<Button>("Delete").clickable.clicked += () => 
        { 
            holder.Remove(goalHolder); 
            levelEditor.RemoveGoal(shapeGoalElement);
            for (int i = 0; i < cells.Count; i++)
                cells[i].RemoveGoal();
        };

        //Grid
        goalHolder.Q<VisualElement>("Holder").Add(GridMaker.MakeGridElement(cells, shapeGoalElement));

        holder.Add(goalHolder);
        cells.Clear();
    }

    public void MakeWithLoadData(Dictionary<CellElement, DotType> mappedValus, VisualElement holder, VisualTreeAsset spawnHolder, LevelEditor levelEditor)
    {
        List<CellElement> allCells = new List<CellElement>(mappedValus.Keys);

        //Set Color for cells
        for (int i = 0; i < allCells.Count; i++)
        {
            allCells[i].ChangeCellColor(CellColorState.partGoal);
            cells.Add(allCells[i]);
        }

        //Setup data
        ShapeGoalElement shapeGoalElement = new ShapeGoalElement();
        levelEditor.shapeGoals.Add(shapeGoalElement);

        //Connect behavior
        VisualElement goalHolder = spawnHolder.Instantiate();
        goalHolder.Q<Button>("Delete").clickable.clicked += () =>
        {
            holder.Remove(goalHolder);
            levelEditor.RemoveGoal(shapeGoalElement);
            for (int i = 0; i < cells.Count; i++)
                cells[i].RemoveGoal();
        };

        //Grid
        goalHolder.Q<VisualElement>("Holder").Add(GridMaker.MakeGridElement(mappedValus, shapeGoalElement));

        holder.Add(goalHolder);
        cells.Clear();
    }
}

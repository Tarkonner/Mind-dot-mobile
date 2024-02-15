using SharedData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlaceGoalElement : VisualElement
{
    public PlaceGoalData placeGoalData;

    public CellElement cellRef { get; private set; }
    private Image sprite;

    public PlaceGoalElement(DotType goalType, CellElement cell)
    {
        placeGoalData.goalType = goalType;
        cellRef = cell;

        sprite = new Image();
        sprite.sprite = Resources.Load<Sprite>("Triangle");
        style.width = 15;
        style.height = 15;

        style.top = 0; 
        style.left = 0;

        style.position = Position.Absolute;
        switch (goalType)
        {
            case DotType.Blue:
                sprite.tintColor = Color.blue;
                break;
            case DotType.Red:
                sprite.tintColor = Color.red;
                break;
            case DotType.Yellow:
                sprite.tintColor = Color.yellow;
                break;
            case DotType.Null:
                sprite.tintColor=Color.grey; 
                break;
            default:
                Debug.LogError("Not set dotType for Dot element");
                break;
        }

        Add(sprite);
    }
    public void ChangeColor()
    {
        switch (placeGoalData.goalType)
        {
            case DotType.Blue:
                placeGoalData.goalType = DotType.Red;
                sprite.tintColor = Color.red;
                break;
            case DotType.Red:
                placeGoalData.goalType = DotType.Yellow;
                sprite.tintColor = Color.yellow;
                break;
            case DotType.Yellow:
                placeGoalData.goalType = DotType.Null;
                sprite.tintColor = Color.grey;
                break;
            case DotType.Null:
                placeGoalData.goalType = DotType.Blue;
                sprite.tintColor = Color.blue;
                break;
        }
    }
    public bool GoalCompletionStatus()
    {
        if (cellRef.cellData.holding != null)
        {
            if (placeGoalData.goalType == DotType.Null || placeGoalData.cellData.holding.dotType == placeGoalData.goalType)
            {
                return true;
            }
        }
        return false;
    }
}

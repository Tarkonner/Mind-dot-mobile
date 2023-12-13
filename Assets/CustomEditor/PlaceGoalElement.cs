using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlaceGoalElement : VisualElement
{
    public DotType goalType;
    public Vector2 goalPosition;
    public CellElement goalCell;
    private Image sprite;

    public PlaceGoalElement(DotType goalType, CellElement cell)
    {
        this.goalType = goalType;
        goalCell = cell;
        goalPosition = cell.gridCoordinates;

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
        switch (goalType)
        {
            case DotType.Blue:
                goalType = DotType.Red;
                sprite.tintColor = Color.red;
                break;
            case DotType.Red:
                goalType = DotType.Yellow;
                sprite.tintColor = Color.yellow;
                break;
            case DotType.Yellow:
                goalType = DotType.Null;
                sprite.tintColor = Color.grey;
                break;
            case DotType.Null:
                goalType = DotType.Blue;
                sprite.tintColor = Color.blue;
                break;
        }
    }
    public bool GoalCompletionStatus()
    {
        if (goalCell.holding!=null)
        {
            if (goalType == DotType.Null || goalCell.holding.dotType == goalType)
            {
                return true;
            }
        }
        return false;
    }
}

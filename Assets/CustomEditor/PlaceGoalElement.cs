using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlaceGoalElement : VisualElement
{
    public DotType goalType;

    private Image sprite;

    public PlaceGoalElement(DotType goalType)
    {
        this.goalType = goalType;

        sprite = new Image();
        sprite.sprite = Resources.Load<Sprite>("Triangle");
        style.width = 15;
        style.height = 15;
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
}

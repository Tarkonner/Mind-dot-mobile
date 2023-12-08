using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DotElement : VisualElement
{
    private Image image;
    public DotType dotType { get; private set; }

    public bool isConnected;

    public DotElement(DotType type) 
    {
        this.dotType = type;

        image = new Image();
        image.sprite = Resources.Load<Sprite>("Circle");
        style.width = 30;
        style.height = 30;

        switch(type) 
        {
            case DotType.Blue:
                image.tintColor = Color.blue;
                break;
            case DotType.Red:
                image.tintColor = Color.red;
                break;
            case DotType.Yellow:
                image.tintColor = Color.yellow;
                break;
            default:
                Debug.LogError("Not set dotType for Dot element");
                break;
        }

        Add(image);
    }

    public void ChangeColor()
    {
        switch(dotType) 
        {
            case DotType.Blue:
                dotType = DotType.Red;
                image.tintColor = Color.red;
                break;
            case DotType.Red:
                dotType = DotType.Yellow;
                image.tintColor = Color.yellow;
                break;
            case DotType.Yellow:
                dotType = DotType.Blue;
                image.tintColor = Color.blue;
                break;
        }
    }
}

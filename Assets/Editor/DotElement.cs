using SharedData;
using UnityEngine;
using UnityEngine.UIElements;

public class DotElement : VisualElement
{
    private Image image;

    public DotData DotData = new DotData();

    public DotElement(DotType type)
    {
        DotData.dotType = type;

        image = new Image();
        image.sprite = Resources.Load<Sprite>("Circle");
        style.width = 30;
        style.height = 30;

        switch (type)
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
            case DotType.Null:
                image.tintColor = Color.gray;
                break;
            default:
                Debug.LogError($"Not set dotType for Dot element. Was: {type}");
                break;
        }

        Add(image);
    }

    public void ChangeColor()
    {
        switch (DotData.dotType)
        {
            case DotType.Blue:
                DotData.dotType = DotType.Red;
                image.tintColor = Color.red;
                break;
            case DotType.Red:
                DotData.dotType = DotType.Yellow;
                image.tintColor = Color.yellow;
                break;
            case DotType.Yellow:
                DotData.dotType = DotType.Blue;
                image.tintColor = Color.blue;
                break;
        }
    }

    public void UpdateColor()
    {
        switch (DotData.dotType)
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
            case DotType.Null:
                image.tintColor = Color.gray;
                break;
        }
    }
}

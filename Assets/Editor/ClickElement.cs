using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ClickElement : Image
{
    public DotElement holdingDot = null;
    private DotType savedDotType;

    public ClickElement() 
    {

        sprite = Resources.Load<Sprite>("GridElementSprite");
        RegisterCallback<MouseDownEvent>(OnMouseDown);
    }

    private void OnMouseDown(MouseDownEvent evt)
    {
        if(holdingDot != null)
        {

            if (holdingDot.DotData.dotType != DotType.Null)
            {
                savedDotType = holdingDot.DotData.dotType;
                holdingDot.DotData.dotType = DotType.Null;
                holdingDot.UpdateColor();
            }
            else
            {
                holdingDot.DotData.dotType = savedDotType;
                holdingDot.UpdateColor();
            }
        }
    }
}

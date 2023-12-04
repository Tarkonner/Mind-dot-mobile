using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PieceElement : GridElement
{
    private bool canRotate = true;

    public PieceElement(LevelEditor editor) : base(editor)
    {
    }

    public void ChangeRotationStatus()
    {
        canRotate = !canRotate;

        if(canRotate)
        {
            foreach (Image i in images)
                i.tintColor = Color.white;
        }
        else
        {
            foreach(Image i in images)
                i.tintColor = Color.cyan;
        }    
    }
}

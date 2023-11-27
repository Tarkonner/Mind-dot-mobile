using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelEditor : MonoBehaviour
{
    public int DrawType = 0;

    private DotType dotType;

    public void ChangeDotType(int targetType)
    {
        Debug.Log(targetType);

        switch (targetType)
        {
            case 0:
                dotType = DotType.Blue;
                break;
            case 1:
                dotType = DotType.Yellow;
                break;
            case 2:
                dotType = DotType.Red;
                break;
        }
    }
       
}

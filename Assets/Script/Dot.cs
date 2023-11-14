using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dot : MonoBehaviour, IOccupying
{
    public DotType dotType;

    private bool isConnected;

    //Consider moving Grid Position storage into here.

    public bool IsConnected { get => isConnected; set => isConnected = value; }

    public void Setup(DotType targetType)
    {
        Image renderer = GetComponent<Image>();
        
        switch (targetType)
        {
            case DotType.Red:
                renderer.color = Color.red;
                break;
            case DotType.Blue:
                renderer.color = Color.blue;
                break;
            case DotType.Yellow:
                renderer.color = Color.yellow;
                break;
        }
    }
}

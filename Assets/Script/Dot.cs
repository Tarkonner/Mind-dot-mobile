using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour, IOccupying
{
    public DotType dotType;

    private bool isConnected;

    //Consider moving Grid Position storage into here.

    public bool IsConnected { get => isConnected; set => isConnected = value; }
}

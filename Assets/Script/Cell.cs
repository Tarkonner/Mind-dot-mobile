using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public List<IPlacable> content;
    public IOccupying occupying = null;
    public Vector2Int gridPos;

    public Action OnPlacement;
}

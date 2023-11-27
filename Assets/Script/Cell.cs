using System;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public List<IPlacable> content;
    public IOccupying occupying = null;
    public Vector2Int gridPos;

    public Action OnPlacement;

    public void ConvertToCell(SerializableCell serializableCell)
    {
        this.content = serializableCell.content;
        this.occupying = serializableCell.occupying;
        this.gridPos = serializableCell.gridPos;
    }
}


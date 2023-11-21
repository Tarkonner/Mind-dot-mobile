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
        this.OnPlacement = serializableCell.OnPlacement;
    }
}

[ES3Serializable]
public class SerializableCell
{
    public List<IPlacable> content;
    public IOccupying occupying = null;
    public Vector2Int gridPos;

    public Action OnPlacement;

    public bool isNullCell;

    public SerializableCell(Cell cell)
    {
        isNullCell = false;

        this.content = cell.content;
        this.occupying = cell.occupying;
        this.gridPos = cell.gridPos;
        this.OnPlacement = cell.OnPlacement;
    }

    // This constructor represents a null cell.
    public SerializableCell()
    {
        isNullCell = true;

        this.content = null;
        this.occupying = null;
        this.gridPos = new Vector2Int();
        this.OnPlacement = null;
    }
}
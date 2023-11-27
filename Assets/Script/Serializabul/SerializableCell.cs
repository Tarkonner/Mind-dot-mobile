using System;
using System.Collections.Generic;
using UnityEngine;

[ES3Serializable]
public class SerializableCell
{
    public List<IPlacable> content;
    public SerializableDot occupying = null;
    public Vector2Int gridPos;

    public bool isNullCell;

    public SerializableCell(Cell cell)
    {
        isNullCell = false;

        this.content = cell.content;
        this.gridPos = cell.gridPos;

        if (cell.occupying is Dot)
        {
            Dot refDot = (Dot)cell.occupying;

            this.occupying = SaveConverter.ConvertToSerializableDot(refDot);
        }
        else
            this.occupying = null;

    }

    // This constructor represents a null cell.
    public SerializableCell()
    {
        isNullCell = true;

        this.content = null;
        this.occupying = null;
        this.gridPos = new Vector2Int();
    }
}
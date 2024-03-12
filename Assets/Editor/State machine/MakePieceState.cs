using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MakePieceState : CollectCells
{
    public void Execute(VisualElement holder, VisualTreeAsset spawnHolder)
    {
        if (cells.Count == 0)
        {
            Debug.Log("No cells selected");
            return;
        }

        holder.Add(spawnHolder.Instantiate());
        
        cells.Clear();
    }
}

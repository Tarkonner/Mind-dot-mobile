using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellEditState : EditorState
{
    public void Execute(CellElement cell)
    {
        cell.TurnOffCell();
    }
}

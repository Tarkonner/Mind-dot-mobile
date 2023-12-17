using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class PlaceGoal : MonoBehaviour
{
    public DotType goalType;
    public Cell cell;

    public PlaceGoal(DotType goalType, Cell cell)
    {
        this.goalType = goalType;
        this.cell = cell;
    }

    public bool CheckFulfilment()
    {
        if (cell.occupying is Dot dot)
        {
            if (goalType==DotType.Null || dot.dotType==goalType)
            {
                cell.gameObject.GetComponent<Image>().color = Color.green;
                return true;
            }
        }
        cell.gameObject.GetComponent<Image>().color = Color.white;
        return false;
    }
    public static void MakeGoal(LevelPlaceGoal levelPlaceGoal, Cell[,] grid)
    {
        Cell cell = grid[(int)levelPlaceGoal.goalPosition.x, (int)levelPlaceGoal.goalPosition.y];
        GameObject newObject = new GameObject();
        newObject.transform.parent = cell.transform;
        Image image = newObject.AddComponent<Image>();
        PlaceGoal pG = newObject.AddComponent<PlaceGoal>();
        pG.cell = cell;
        pG.goalType = levelPlaceGoal.type;
    }
}

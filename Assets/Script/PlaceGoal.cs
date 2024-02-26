using UnityEngine;
using UnityEngine.UI;

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
            if (goalType == DotType.Null || dot.dotType == goalType)
            {
                cell.gameObject.GetComponent<Image>().color = Color.green;
                return true;
            }
        }
        cell.gameObject.GetComponent<Image>().color = Color.white;
        return false;
    }

    public void MakeGoal(LevelPlaceGoal levelPlaceGoal, Cell[,] grid)
    {
        Cell cell = grid[(int)levelPlaceGoal.goalPosition.x, (int)levelPlaceGoal.goalPosition.y];

        Image image = GetComponent<Image>();
        PlaceGoal pG = GetComponent<PlaceGoal>();
        image.rectTransform.localPosition = new Vector3(-image.rectTransform.sizeDelta.x, image.rectTransform.sizeDelta.y, 0);
        pG.cell = cell;
        pG.goalType = levelPlaceGoal.type;
        switch (levelPlaceGoal.type)
        {
            case DotType.Null:
                image.color = Color.gray;
                break;
            case DotType.Blue:
                image.color = Color.blue;
                break;
            case DotType.Red:
                image.color = Color.red;
                break;
            case DotType.Yellow:
                image.color = Color.yellow;
                break;
        }
    }
}

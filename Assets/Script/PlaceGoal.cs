using UnityEngine;
using UnityEngine.UI;

public class PlaceGoal : MonoBehaviour
{
    [SerializeField] ColorBank colorBank;
    public DotType goalType;
    public Cell cell;

    public bool CheckFulfilment()
    {
        if (cell.occupying is Dot dot)
        {
            if (goalType == DotType.Null || dot.dotType == goalType)
            {
                return true;
            }
        }
        return false;
    }

    public void MakeGoal(LevelPlaceGoal levelPlaceGoal, Cell[,] grid)
    {
        Cell cell = grid[(int)levelPlaceGoal.goalPosition.x, (int)levelPlaceGoal.goalPosition.y];

        Image image = GetComponent<Image>();
        PlaceGoal pG = GetComponent<PlaceGoal>();
        image.rectTransform.localPosition = new Vector3(0, 0, 0);
        pG.cell = cell;
        pG.goalType = levelPlaceGoal.type;
        switch (levelPlaceGoal.type)
        {
            case DotType.Null:
                image.color = Color.white;
                Board.Instance.grid[cell.gridPos.x, cell.gridPos.y].GetComponent<Image>().color = colorBank.backgroundNull;
                break;
            case DotType.Blue:
                image.color = colorBank.blueColor;
                Board.Instance.grid[cell.gridPos.x, cell.gridPos.y].GetComponent<Image>().color = colorBank.backgroundBlueColor;
                break;
            case DotType.Red:
                image.color = colorBank.redColor;
                Board.Instance.grid[cell.gridPos.x, cell.gridPos.y].GetComponent<Image>().color = colorBank.backgroundRedColor;
                break;
            case DotType.Yellow:
                image.color = colorBank.yellowColor;
                Board.Instance.grid[cell.gridPos.x, cell.gridPos.y].GetComponent<Image>().color = colorBank.backgroundYellowColor;
                break;
        }
    }
}

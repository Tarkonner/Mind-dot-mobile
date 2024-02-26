using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeGoal : MonoBehaviour, IGoal
{
    [SerializeField] GameObject dotPrefab;
    [SerializeField] float spaceingBetweenDots = 100;

    private int goalsizeX;
    private int goalsizeY;

    int goalMaxX;
    int goalMaxY;

    public bool completed { get; private set; } = false;

    private List<Dot> goalsDots = new List<Dot>();
    private List<Vector2> dotCoordinats = new List<Vector2>();  

    private Image background;
    private Color uncompletedColor;
    private Color completedColor;

    public void LoadGoal(LevelShapeGoal targetGoal)
    {
        for (int i = 0; i < targetGoal.goalSpecifications.Length; i++)
        {
            //Set position
            Vector2 offset = new Vector2((targetGoal.goalSize.x - 1) * 0.5f, (targetGoal.goalSize.y - 1) * 0.5f);
            GameObject spawnedDot = Instantiate(dotPrefab, transform);
            RectTransform dotRec = spawnedDot.GetComponent<RectTransform>();
            dotRec.anchoredPosition = new Vector2(
                targetGoal.goalSpecifications[i].x * spaceingBetweenDots - offset.x * spaceingBetweenDots,
                targetGoal.goalSpecifications[i].y * spaceingBetweenDots - offset.y * spaceingBetweenDots);

            //Find highets & lowest
            if (targetGoal.goalSpecifications[i].x > goalMaxX)
                goalMaxX = (int)targetGoal.goalSpecifications[i].x;
            if (targetGoal.goalSpecifications[i].y > goalMaxY)
                goalMaxY = (int)targetGoal.goalSpecifications[i].y;

            //Setup dot
            Dot d = spawnedDot.GetComponent<Dot>();
            d.Setup(targetGoal.goalDots[i]);
            goalsDots.Add(d);
            dotCoordinats.Add(new Vector2(targetGoal.goalSpecifications[i].x, targetGoal.goalSpecifications[i].y));
        }
    }

    void Start()
    {
        goalsizeX = goalMaxX + 1;
        goalsizeY = goalMaxY + 1;

        RectTransform goalRect = gameObject.GetComponent<RectTransform>();
        //The dots need to be scaled according to how many can fit within the square
        //The +1 is to account for the 0-indexed grid position.
        int detSize = Mathf.Max(new int[] { goalsizeX, goalsizeY });
        for (int i = 0; i < goalsDots.Count; i++)
        {
            RectTransform dotRect = goalsDots[i].GetComponent<RectTransform>();
            dotRect.sizeDelta = goalRect.sizeDelta / detSize;
            dotRect.anchoredPosition = goalRect.anchoredPosition + (dotCoordinats[i] -
                new Vector2(goalsizeX / 2 - 0.5f * ((goalsizeX + 1) % 2), goalsizeY / 2 - 0.5f * ((goalsizeY + 1) % 2))) * dotRect.sizeDelta;
        }

        background = GetComponent<Image>();
        if (uncompletedColor == Color.clear)
        {
            uncompletedColor = new Color(0.75f, 0.75f, 0.75f, 1);
        }
        if (completedColor == Color.clear)
        {
            completedColor = new Color(0.1f, 0.9f, 0.2f, 1);
        }
        background.color = uncompletedColor;
    }

    public bool CheckFulfilment(Board board)
    {
        for (int x = 0; x < board.grid.GetLength(0); x++)
        {
            for (int y = 0; y < board.grid.GetLength(1); y++)
            {
                if (board.grid[x, y] == null) 
                    continue;

                if (board.grid[x, y].occupying is Dot currentDot && 
                    currentDot.dotType == goalsDots[0].dotType)
                {
                    if (CheckForPatternAtPosition(board, new Vector2(x, y)))
                    {
                        completed = true;
                        background.color = completedColor;
                        return true;
                    }
                }
            }
        }
        completed = false;
        background.color = uncompletedColor;
        return false;
    }
    private bool CheckForPatternAtPosition(Board board, Vector2 currentPos)
    {
        for (int i = 0; i < goalsDots.Count; i++)
        {
            Vector2 assumedPos = new Vector2(currentPos.x, currentPos.y) + (dotCoordinats[i]);
            if ((int)assumedPos.x > board.grid.GetLength(0) - 1 || (int)assumedPos.y > board.grid.GetLength(1) - 1) 
                return false;
            if (board.grid[(int)assumedPos.x, (int)assumedPos.y] == null) 
                return false;

            if (board.grid[(int)assumedPos.x, (int)assumedPos.y].occupying is Dot checkDot &&
                checkDot.dotType == goalsDots[i].dotType)
            {

            }
            else
            {
                return false;
            }
        }
        return true;
    }
}

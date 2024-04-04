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

    [Header("Color")]
    private Image background;
    [SerializeField] private Color uncompletedColor;
    [SerializeField] private Color completedColor;

    public void LoadGoal(LevelShapeGoal targetGoal)
    {
        for (int i = 0; i < targetGoal.goalSpecifications.Length; i++)
        {
            //Find highets & lowest
            if (targetGoal.goalSpecifications[i].x > goalMaxX)
                goalMaxX = (int)targetGoal.goalSpecifications[i].x;
            if (targetGoal.goalSpecifications[i].y > goalMaxY)
                goalMaxY = (int)targetGoal.goalSpecifications[i].y;

            //Setup dot
            GameObject spawnedDot = Instantiate(dotPrefab, transform);
            Dot d = spawnedDot.GetComponent<Dot>();
            d.Setup(targetGoal.goalDots[i]);
            goalsDots.Add(d);
            dotCoordinats.Add(new Vector2(targetGoal.goalSpecifications[i].x, targetGoal.goalSpecifications[i].y));

            //Transform
            //position
            Vector2 offset = new Vector2((targetGoal.goalSize.x - 1) * 0.5f, (targetGoal.goalSize.y - 1) * 0.5f);
            RectTransform dotRec = spawnedDot.GetComponent<RectTransform>();
            dotRec.anchoredPosition = new Vector2(
                targetGoal.goalSpecifications[i].x * spaceingBetweenDots - offset.x * spaceingBetweenDots,
                targetGoal.goalSpecifications[i].y * spaceingBetweenDots - offset.y * spaceingBetweenDots);
            //Scale
            goalsizeX = goalMaxX + 1;
            goalsizeY = goalMaxY + 1;
            RectTransform goalRect = gameObject.GetComponent<RectTransform>();
            //The dots need to be scaled according to how many can fit within the square
            //The +1 is to account for the 0-indexed grid position.
            int detSize = Mathf.Max(new int[] { goalsizeX, goalsizeY });
            for (int h = 0; h < goalsDots.Count; h++)
            {
                RectTransform dotRect = goalsDots[h].GetComponent<RectTransform>();
                dotRect.sizeDelta = goalRect.sizeDelta / detSize;
                dotRect.anchoredPosition = goalRect.anchoredPosition + 
                    (dotCoordinats[h] - new Vector2(
                        goalsizeX / 2 - 0.5f * ((goalsizeX + 1) % 2), 
                        goalsizeY / 2 - 0.5f * ((goalsizeY + 1) % 2))) * dotRect.sizeDelta;
            }

            //Placeholder for mirror problem around x-axis
            transform.localEulerAngles = new Vector3(180, 0, 0);
        }
    }

    private void Awake()
    {
        //Background
        background = GetComponent<Image>();
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
        Debug.Log("Check");

        Vector2 assumePos = currentPos;

        for (int i = 0; i < goalsDots.Count; i++)
        {
            if (i != 0)
            {
                Vector2 dif = dotCoordinats[i] - dotCoordinats[i - 1];
                Debug.Log("Dif: " + dif);
                assumePos = assumePos + new Vector2(dif.x, dif.y);
            }

            Debug.Log("Regnet: " + assumePos);

            if ((int)assumePos.x > board.grid.GetLength(0) - 1 || (int)assumePos.y > board.grid.GetLength(1) - 1
                || assumePos.x < 0 || assumePos.y < 0)
            {
                Debug.Log("Out off bounds");
                return false;
            }
            if (board.grid[(int)assumePos.x, (int)assumePos.y] == null)
            {
                Debug.Log("A");
                return false;
            }

            if (board.grid[(int)assumePos.x, (int)assumePos.y].occupying is Dot checkDot)
            {
                Debug.Log(checkDot.dotType);
                if (goalsDots[i].dotType == DotType.Null)
                {
                    Debug.Log("Null dot");
                }
                else if (checkDot.dotType != goalsDots[i].dotType)
                    return false;
            }
            else
                return false;
        }
        return true;
    }
}

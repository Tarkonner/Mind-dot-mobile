using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class LevelManager : MonoBehaviour
{
    [Header("Refences")]
    [SerializeField] Board board;
    [SerializeField] PieceMaker pieceHolder;
    [SerializeField] GoalMaker goalMaker;
    [SerializeField] LevelText levelText;

    [Header("Animations")]
    [SerializeField] float completedLevelPauseTime = .5f;
    [SerializeField] float sizeAnimationTime = .5f;
    [SerializeField] float dotsBonusSize = .15f;

    [Header("Goals")]
    [SerializeField] private GameObject goalHolder;
    [SerializeField] private LevelSO[] levels;
    private List<IGoal> allGoals = new List<IGoal>();


    [Header("Testing")]
    [SerializeField] bool loadTestlevel = false;
    [SerializeField] private LevelSO testLevel;

    //Analytics
    private float timeForCompletingLevels;

    public int targetLevel { get; private set; } = 0;
    public LevelSO currentLevel { get; private set; }

    //Events
    public delegate void OnLoadLevel();
    public static event OnLoadLevel onLoadLevel;
    public delegate void OnDeloadLevel();
    public static event OnDeloadLevel onDeloadLevel;

    private void Start()
    {
        //Load level
#if (UNITY_EDITOR)
        if (loadTestlevel && testLevel != null)
        {
            LoadLevel(testLevel);
            return;
        }
#endif
        //Load first level
        LoadLevel(levels[targetLevel]);

        //UI show level index
        levelText.LevelIndex(targetLevel);
    }

    private void OnEnable()
    {
        InputSystem.onDotChange += GoalProgression;
    }
    private void OnDisable()
    {
        InputSystem.onDotChange -= GoalProgression;
    }

    private void Update()
    {
        timeForCompletingLevels += Time.deltaTime;
    }

    public void LoadLevel(LevelSO targetLevel)
    {
        currentLevel = targetLevel;

        onLoadLevel?.Invoke();

        //Clear old
        allGoals.Clear();

        board.LoadLevel(targetLevel); //Uses info from both board & pieces, so piece dots don't get loadet in
        goalMaker.MakeGoals(targetLevel);
        pieceHolder.MakePieces(targetLevel.levelPieces);
    }

    public void GoalProgression()
    {
        //See how many goals
        if (allGoals.Count == 0)
        {
            for (int i = 0; i < goalMaker.holder.childCount; i++)
            {
                IGoal check = goalMaker.holder.GetChild(i).GetComponent<IGoal>();

                if (check != null)
                {
                    allGoals.Add(check);
                }
            }
        }

        int completedGoals = 0;
        foreach (var child in allGoals)
        {
            if (child.CheckFulfilment(board))
            {
                completedGoals++;
            }
        }
        if (completedGoals == allGoals.Count)
        {
            //Tell test levels
#if (UNITY_EDITOR)
            if (loadTestlevel)
            {
                Debug.Log("Complete level");
                return;
            }
#endif

            //Get Dots from completet goals
            List<Dot> collectetDots = new List<Dot>();
            for (int i = 0; i < allGoals.Count; i++)
            {
                if (allGoals[i] is ShapeGoal)
                {
                    ShapeGoal sg = (ShapeGoal)allGoals[i];
                    for (int j = 0; j < sg.lastCheckedDots.Count; j++)
                        collectetDots.Add(sg.lastCheckedDots[j]);
                }
            }
            //Animate
            for (int i = 0; i < collectetDots.Count; i++)
            {
                float targetScale = collectetDots[i].transform.localScale.x + dotsBonusSize;
                collectetDots[i].transform.DOScale(targetScale, sizeAnimationTime);
            }

            //Analytics
            Dictionary<string, object> LevelData = new Dictionary<string, object>()
            {
                {"0", targetLevel },
                {"1", timeForCompletingLevels }
            };
            Analytics.CustomEvent("LevelComplete", LevelData);
            timeForCompletingLevels = 0;

            //Load level
            targetLevel++;
            if (targetLevel == levels.Length)
            {
                Debug.Log("All levels complete");
                targetLevel = 0;

                onDeloadLevel?.Invoke();
                LoadLevel(levels[targetLevel]);
                levelText.LevelIndex(targetLevel);
            }
            else
            {
                Debug.Log("Level Complete");
                StartCoroutine(WinPause());
            }
        }
    }

    IEnumerator WinPause()
    {
        yield return new WaitForSeconds(completedLevelPauseTime);
        LoadNextLevel();
    }

    private void LoadNextLevel()
    {
        onDeloadLevel?.Invoke();
        LoadLevel(levels[targetLevel]);
        levelText.LevelIndex(targetLevel);
    }
}
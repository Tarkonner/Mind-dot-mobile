using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    LevelManager levelManager;

    [SerializeField] int[] tutorialIndex;
    [SerializeField] GameObject[] tutorialObjects;

    private void Awake()
    {
        levelManager = GetComponent<LevelManager>();

        LevelManager.onLoadLevel += CheckForTutorial;
    }

    void CheckForTutorial()
    {
        for (int i = 0; i < tutorialObjects.Length; i++)
            tutorialObjects[i].SetActive(false);

        for (int i = 0; i < tutorialIndex.Length; i++)
        {
            if (tutorialIndex[i] == levelManager.targetLevel)
                LoadTutorial(tutorialIndex[i]);
        }
    }

    void LoadTutorial(int targetTutorial)
    {
        tutorialObjects[targetTutorial].SetActive(true);
    }
}

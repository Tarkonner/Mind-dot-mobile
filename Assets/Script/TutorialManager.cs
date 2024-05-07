using System;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    LevelManager levelManager;

    [SerializeField] LevelSO[] levelIndex;
    [SerializeField] InspectorGameobject[] tutorialObjects;
    private InspectorGameobject currentTutorialObjects;

    private void Awake()
    {
        levelManager = GetComponent<LevelManager>();

        LevelManager.onLoadLevel += LoadTutorial;
        LevelManager.onDeloadLevel += RemoveTutorial;
    }

    void RemoveTutorial()
    {
        for (int i = currentTutorialObjects.gameObjects.Length - 1; i >= 0; i--)
        {
            currentTutorialObjects.gameObjects[i].SetActive(false);
        }
    }

    void LoadTutorial()
    {
        for (int i = 0; i < levelIndex.Length; i++)
        {
            if (levelIndex[i] == levelManager.currentLevel)
            {
                currentTutorialObjects = tutorialObjects[i];
                break;
            }
        }

        if (currentTutorialObjects != null)
        {
            for (int i = 0; i < currentTutorialObjects.gameObjects.Length; i++)
            {
                currentTutorialObjects.gameObjects[i].SetActive(true);
            }
        }
    }
}

[Serializable]
class InspectorGameobject
{
    public GameObject[] gameObjects;
}
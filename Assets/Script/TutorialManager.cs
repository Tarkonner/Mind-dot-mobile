using System;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    LevelManager levelManager;
    [SerializeField] TextMeshProUGUI textMesh;
    private GameObject textParent;

    [SerializeField] LevelSO[] levelIndex;
    [SerializeField] InspectorGameobject[] tutorialObjects;
    private InspectorGameobject currentTutorialObjects;

    private void Start()
    {
        levelManager = GetComponent<LevelManager>();

        LevelManager.onLoadLevel += LoadTutorial;
        LevelManager.onDeloadLevel += RemoveTutorial;

        textParent = textMesh.transform.parent.gameObject;
    }

    void RemoveTutorial()
    {
        textParent.SetActive(false);

        if(currentTutorialObjects != null) 
        {
            for (int i = currentTutorialObjects.gameObjects.Length - 1; i >= 0; i--)
            {
                currentTutorialObjects.gameObjects[i].SetActive(false);
            }
        }
    }

    void LoadTutorial()
    {
        //Find specifik toturial asset
        for (int i = 0; i < levelIndex.Length; i++)
        {
            if (levelIndex[i] == levelManager.currentLevel)
            {
                currentTutorialObjects = tutorialObjects[i];
                break;
            }
            else
                currentTutorialObjects = null;
        }

        if (currentTutorialObjects != null)
        {
            textParent.SetActive(true);
            textMesh.text = currentTutorialObjects.levelText;

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
    public string levelText;
    public GameObject[] gameObjects;
}

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    LevelManager levelManager;    
    [SerializeField] TextMeshProUGUI textMesh;
    [SerializeField] InspectorGameobject[] tutorialObjects;
    private InspectorGameobject currentTutorialObjects;

    [Header("Text animation")]
    [SerializeField] ScaleAnimations animations;
    [SerializeField] GameObject textHolder;
    [SerializeField] float scaleInTime = .3f;
    private List<GameObject> toAnimate;


    private void Awake()
    {
        levelManager = GetComponent<LevelManager>();

        toAnimate = new List<GameObject>() { textHolder };
    }

    private void OnEnable()
    {
        LevelManager.onLoadLevel += LoadTutorial;
        LevelManager.onDeloadLevel += RemoveTutorial;
    }

    private void OnDisable()
    {
        LevelManager.onLoadLevel -= LoadTutorial;
        LevelManager.onDeloadLevel -= RemoveTutorial;
    }

    void RemoveTutorial()
    {
        animations.ScaleOutLiniar(toAnimate, scaleInTime);

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
        for (int i = 0; i < tutorialObjects.Length; i++)
        {
            if (tutorialObjects[i].targetLevel == levelManager.currentLevel)
            {
                currentTutorialObjects = tutorialObjects[i];
                break;
            }
            else
                currentTutorialObjects = null;
        }

        if (currentTutorialObjects != null)
        {
            animations.ScaleInLiniar(toAnimate, scaleInTime);
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
    public LevelSO targetLevel;
    public string levelText;
    public GameObject[] gameObjects;
}

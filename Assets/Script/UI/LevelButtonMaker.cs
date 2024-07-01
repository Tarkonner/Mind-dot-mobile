using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButtonMaker : MonoBehaviour
{
    [SerializeField] LevelsBank levelsBank;
    [Header("Placeing level buttons")]
    [SerializeField] int horizontalElements = 4;
    [SerializeField] int verticalElements = 4;
    [Header("Buttons setup")]
    [SerializeField] GameObject levelButtonPrefab;
    [SerializeField] float spaceBetweenButtons = 100;
    [SerializeField] GameObject levelHolder;
    [Header("Panels")]
    private int numberOfPanels = 3;
    [SerializeField] GameObject leftArrowButton;
    [SerializeField] GameObject rightArrowButton;
    [SerializeField] float spaceBetweenPanels = 900;
    [SerializeField] float slideAnimationTime = .5f;
    private List<Transform> holders = new List<Transform>();
    private LevelSelectButton[,] buttonsBank;
    private int panelCount = 0;
    private int maxPanels;
    [Header("Text")]
    [SerializeField] TextMeshProUGUI textMesh;
    [Header("Color")]
    [SerializeField] ColorBank colorBank;


    void Start()
    {
        //Arrow buttons
        leftArrowButton.SetActive(false);

        maxPanels = (int)MathF.Ceiling((float)levelsBank.levels.Length / (horizontalElements * verticalElements)) - 1;

        //Level
        int targetLevel = 0;

        Vector2 spawnPoint = new Vector2(-spaceBetweenButtons * horizontalElements / 2 + spaceBetweenButtons / 2, spaceBetweenButtons * verticalElements / 2);

        //How many buttons
        buttonsBank = new LevelSelectButton[numberOfPanels, horizontalElements * verticalElements];

        for (int j = 0; j < numberOfPanels; j++)
        {
            //Holder
            GameObject hold = new GameObject($"{j} panel");
            hold.transform.SetParent(levelHolder.transform);
            hold.transform.localScale = Vector3.one;
            holders.Add(hold.transform);
            hold.transform.localPosition = new Vector2(j * spaceBetweenPanels, 0);

            //Spawn buttons
            for (int y = 0; y < verticalElements; y++)
            {
                for (int x = 0; x < horizontalElements; x++)
                {
                    //No level made
                    if (targetLevel > levelsBank.levels.Length)
                        break;

                    //Load level when clicket
                    GameObject spawn = Instantiate(levelButtonPrefab, holders[j]);
                    LevelSelectButton lsb = spawn.GetComponent<LevelSelectButton>();
                    lsb.TargetLevel(targetLevel); //Set level and text
                    Button button = spawn.GetComponent<Button>();
                    button.onClick.AddListener(lsb.LoadLevel);
                    //Save button to later
                    buttonsBank[j, y * verticalElements + x] = lsb;

                    //Set to next level
                    targetLevel++;

                    //Placement
                    Vector2 targetTransform = spawnPoint + new Vector2(x * spaceBetweenButtons, y * -spaceBetweenButtons);
                    button.transform.localPosition = targetTransform;

                    //Give color
                    Image outerImage = spawn.GetComponent<Image>();
                    switch (targetLevel % 3)
                    {
                        case 0:
                            outerImage.color = colorBank.backgroundRedColor;
                            break;
                        case 1:
                            outerImage.color = colorBank.backgroundBlueColor; 
                            break;
                        case 2:
                            outerImage.color = colorBank.backgroundYellowColor; 
                            break;
                    }
                    Image innerImage = spawn.transform.GetChild(0).gameObject.GetComponentInChildren<Image>();
                    switch (targetLevel % 3)
                    {
                        case 0:
                            innerImage.color = colorBank.redColor;
                            break;
                        case 1:
                            innerImage.color = colorBank.blueColor;
                            break;
                        case 2:
                            innerImage.color = colorBank.yellowColor;
                            break;
                    }
                }
            }
        }
    }

    public void MovePanels(bool right)
    {       
        if (right)
        {
            if (panelCount == maxPanels)
                return;

            int targetPanel = (panelCount + 2) % numberOfPanels;

            //Move
            foreach (Transform target in holders)
            {
                target.DOLocalMove(target.localPosition - new Vector3(spaceBetweenPanels, 0, 0), slideAnimationTime).OnComplete(() =>
                {
                    //Move panel to back
                    holders[targetPanel].transform.localPosition = new Vector2(spaceBetweenPanels, 0);
                });
            }

            panelCount++;
            //Updata level load
            if (panelCount > 0)
                UpdatePanelsButton(targetPanel, true);

            //Turn arrow off
            if (panelCount == maxPanels)
                rightArrowButton.SetActive(false);
            
            leftArrowButton.SetActive(true);
        }
        else
        {
            //Stop moveing beond 0
            if (panelCount == 0)
                return;


            //Move
            int targetPanel = (panelCount + 1) % numberOfPanels;
            foreach (Transform target in holders)
            {
                target.DOLocalMove(target.localPosition + new Vector3(spaceBetweenPanels, 0, 0), slideAnimationTime).OnComplete(() =>
                {
                    //Move to front
                    holders[targetPanel].transform.localPosition = new Vector2(-spaceBetweenPanels, 0);
                });
            }         
            panelCount--;
            
            //Updata level load
            UpdatePanelsButton(targetPanel, false);



            //Turn arrow off
            if (panelCount == 0)
                leftArrowButton.SetActive(false);

            rightArrowButton.SetActive(true);
        }

        //Update text
        int numberOfLevels = panelCount * horizontalElements * verticalElements;
        textMesh.text = $"Level {numberOfLevels + 1} - {numberOfLevels + numberOfLevels}";
    }

    void UpdatePanelsButton(int targetPanel, bool right)
    {
        for (int y = 0; y < verticalElements; y++)
        {     
            for (int x = 0; x < horizontalElements; x++)
            {
                int count = y * verticalElements + x;

                int movementModefier = -1;
                if (right)
                    movementModefier = 1;
                int cal = ((panelCount + movementModefier) * horizontalElements * verticalElements) + count;
                buttonsBank[targetPanel, count].TargetLevel(cal);
            }
        }
    }
}

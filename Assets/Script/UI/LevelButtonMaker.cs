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
    private Image[,] outerImages;
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

        //Save resused asset;
        buttonsBank = new LevelSelectButton[numberOfPanels, horizontalElements * verticalElements];
        outerImages = new Image[numberOfPanels, horizontalElements * verticalElements];

        //Make button
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

                    int count = y * verticalElements + x;

                    //Load level when clicket
                    GameObject spawn = Instantiate(levelButtonPrefab, holders[j]);
                    LevelSelectButton lsb = spawn.GetComponent<LevelSelectButton>();
                    lsb.TargetLevel(targetLevel); //Set level and text
                    Button button = spawn.GetComponent<Button>();
                    button.onClick.AddListener(lsb.LoadLevel);
                    //Save button to later
                    buttonsBank[j, count] = lsb;

                    //Check for save
                    bool levelcompletionState = false;
                    if (ES3.FileExists())
                    {
                        string key = SaveSystem.levelKey + targetLevel.ToString();
                        levelcompletionState = ES3.Load<bool>(key);
                    }

                    //Set to next level
                    targetLevel++;

                    //What color
                    switch (targetLevel % 3)
                    {
                        case 0:
                            lsb.dotType = DotType.Red;
                            break;
                        case 1:
                            lsb.dotType = DotType.Blue;
                            break;
                        case 2:
                            lsb.dotType = DotType.Yellow;
                            break;
                    }

                    //Placement
                    Vector2 targetTransform = spawnPoint + new Vector2(x * spaceBetweenButtons, y * -spaceBetweenButtons);
                    button.transform.localPosition = targetTransform;



                    //Give color
                    outerImages[j, count] = spawn.GetComponent<Image>();
                    OuterRingLook(levelcompletionState, j, y * verticalElements + x);


                    Image innerImage = spawn.transform.GetChild(0).gameObject.GetComponentInChildren<Image>();
                    switch (lsb.dotType)
                    {
                        case DotType.Red:
                            innerImage.color = colorBank.redColor;
                            break;
                        case DotType.Blue:
                            innerImage.color = colorBank.blueColor;
                            break;
                        case DotType.Yellow:
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

    void OuterRingLook(bool levelCompletet, int panel, int targetLevel)
    {
        if (levelCompletet)
        {
            outerImages[panel, targetLevel].color = Color.black;
        }
        else
        {
            switch (buttonsBank[panel, targetLevel].dotType)
            {
                case DotType.Red:
                    outerImages[panel, targetLevel].color = colorBank.backgroundRedColor;
                    break;
                case DotType.Blue:
                    outerImages[panel, targetLevel].color = colorBank.backgroundBlueColor;
                    break;
                case DotType.Yellow:
                    outerImages[panel, targetLevel].color = colorBank.backgroundYellowColor;
                    break;
            }
        }
    }
}

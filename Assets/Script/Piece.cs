using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Piece : MonoBehaviour, IDragHandler
{
    [SerializeField] private GameObject dotPrefab;
    RectTransform rectTransform;

    public Vector2[] gridPosArray;
    public Dot[] dotsArray;
    public Vector2 pivotPoint;

    //Rotation
    private bool rotatable = true;
    private int rotationInt = 0;

    [Header("Lines")]
    [SerializeField] private float dotSpacing;
    private List<UILine> connections = new List<UILine>();
    private Transform pieceHolder;
    private GameObject lineHolder;
    public float lineWidth = 10;

    public bool testRotate;

    private Vector2Int savedCenterCoordinats;
    public Vector2Int pieceCenter;
    
    Dictionary<GameObject, Vector2> dotsPosition = new Dictionary<GameObject, Vector2>();


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Start()
    {
        pieceHolder = transform.parent;
    }

    public void LoadPiece(LevelPiece targetPiece)
    {
        //Lines
        lineHolder = new GameObject();
        lineHolder.transform.SetParent(transform, false);

        //Set cordinats
        gridPosArray = new Vector2[targetPiece.dotPositions.Length];
        for (int i = 0; i < gridPosArray.Length; i++)
            gridPosArray[i] = targetPiece.dotPositions[i];

        //Setup dots
        dotsArray = new Dot[targetPiece.dotPositions.Length];
        for (int i = 0; i < targetPiece.dotPositions.Length; i++)
        {
            //Make object
            GameObject spawn = Instantiate(dotPrefab, transform);
            Dot targetDot = spawn.GetComponent<Dot>();
            RectTransform rect = spawn.GetComponent<RectTransform>();

            Vector2 offset = new Vector2((targetPiece.pieceSize.x - 1) * 0.5f, (targetPiece.pieceSize.y - 1) * 0.5f);

            Vector2 calPosition = new Vector2(gridPosArray[i].x * dotSpacing - offset.x * dotSpacing, gridPosArray[i].y * dotSpacing - offset.y * dotSpacing);
            rect.anchoredPosition = calPosition;
            dotsPosition.Add(spawn, calPosition);

            dotsArray[i] = targetDot;
            targetDot.Setup(targetPiece.dotTypes[i], this);
        }

        //Goes through each dot and measures grid distance to each other dot.
        // Distance is used to differentiate adjacent and diagonal dot connections. 
        for (int i = 0; i < gridPosArray.Length; i++)
        {
            if (!dotsArray[i].IsConnected)
            {
                List<int> diagonalList = new List<int>();
                bool foundAdjacent = false;
                for (int j = 0; j < gridPosArray.Length; j++)
                {
                    if (i == j) continue;

                    float val = Vector2.Distance(gridPosArray[i], gridPosArray[j]);
                    if (val > 1.5f) continue;

                    else if (val > 1.2f && !foundAdjacent)
                    {
                        diagonalList.Add(j);
                    }
                    else if (val == 1)
                    {
                        foundAdjacent = true;
                        CreateLine(dotsArray[i], dotsArray[j]);
                    }
                }
                if (!foundAdjacent)
                {
                    foreach (int j in diagonalList)
                    {
                        CreateLine(dotsArray[i], dotsArray[j]);
                    }
                }
            }
        }

        //Find center
        savedCenterCoordinats.x = (int)Mathf.Floor(targetPiece.pieceSize.x / 2);
        savedCenterCoordinats.y = (int)Mathf.Floor(targetPiece.pieceSize.y / 2);
        if (targetPiece.pieceSize.x % 2 == 0) //Even
        {
            savedCenterCoordinats.x = (int)Mathf.Floor(targetPiece.pieceSize.x / 2);
        }
        else //Odd
        {
            savedCenterCoordinats.x = (int)(targetPiece.pieceSize.x / 2);
        }
        if (targetPiece.pieceSize.y % 2 == 0) //Even
        {
            savedCenterCoordinats.y = (int)Mathf.Floor(targetPiece.pieceSize.y / 2);
        }
        else //Odd
        {
            savedCenterCoordinats.y = (int)(targetPiece.pieceSize.y / 2);
        }
        CenterCalculation();

        //Rotation
        for (int i = 0; i < targetPiece.startRotation; i++)
            Rotate();
    }

    public void EnforceDotPositions()
    {
        for (int i = 0;i < dotsArray.Length; i++)
        {
            RectTransform rect = dotsArray[i].GetComponent<RectTransform>();
            //Set position
            rect.anchoredPosition = dotsPosition[dotsArray[i].gameObject];
            //Set rotation
            SetRotation();
        }
    }

    public void Rotate()
    {
        if (!rotatable)
            return;

        for (int i = 0; i < gridPosArray.Length; i++)
        {
            //Rotation math for vector rotation around a point. (x,y)->(x',y')=(a+(x-a)cos(t)-(y-?)sin(t),b+(x-a)sin(t)+(y-b)cos(t))
            //(x,y) is the point that is to be rotated. (a,b) is the pivot point of the rotation.
            //Since we want a 90 degree rotation, the formula effectively becomes: (x,y)?(??(y??),?+(x??))
            gridPosArray[i] = new Vector2((pivotPoint.x + (gridPosArray[i].y - pivotPoint.y)),
               (pivotPoint.y - (gridPosArray[i].x - pivotPoint.y)));           
        }

        rotationInt = (rotationInt + 1) % 4;
        CenterCalculation();

        //Set rotation
        SetRotation();
    }

    public void CreateLine(Dot dot1, Dot dot2)
    {
        //Setup
        GameObject newObject = new GameObject();
        newObject.AddComponent<CanvasRenderer>();
        RectTransform newRectTrans = newObject.AddComponent<RectTransform>();

        //Get values
        newRectTrans.SetParent(lineHolder.transform, false);
        RectTransform dot1Rect = dot1.GetComponent<RectTransform>();
        RectTransform dot2Rect = dot2.GetComponent<RectTransform>();

        //Line
        UILine uiLine = newObject.AddComponent<UILine>();
        connections.Add(uiLine);
        uiLine.Initialzie(dot1Rect, dot2Rect, lineWidth);
        dot1.IsConnected = true;
        dot2.IsConnected = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = InputSystem.instance.touchPosition;
    }

    public void ReturnToHolder()
    {
        gameObject.transform.SetParent(pieceHolder);
        SmallScale();
        rectTransform.anchoredPosition = Vector2.zero;
    }

    public void SmallScale()
    {
        transform.localScale = new Vector3(.5f, .5f, .5f);
    }

    public void NormalScale()
    {
        transform.localScale = Vector3.one;
    }

    private void SetRotation() => GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, rotationInt * -90);

    private void CenterCalculation()
    {
        switch (rotationInt)
        {
            case 0:
                pieceCenter = new Vector2Int(-savedCenterCoordinats.x, savedCenterCoordinats.y);
                break;
            case 1:
                pieceCenter = new Vector2Int(-savedCenterCoordinats.y, -savedCenterCoordinats.x);
                break;
            case 2:
                pieceCenter = new Vector2Int(savedCenterCoordinats.x, -savedCenterCoordinats.y);
                break;
            case 3:
                pieceCenter = new Vector2Int(savedCenterCoordinats.y, savedCenterCoordinats.x);
                break;
        }
    }
}

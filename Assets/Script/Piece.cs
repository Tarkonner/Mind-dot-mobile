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

    [SerializeField] private float dotSpacing;

    private List<UILine> connections = new List<UILine>();

    private bool rotatable = true;

    private int rotationInt = 0;

    public GameObject testPivotPoint;
    private Transform pieceHolder;
    private GameObject lineHolder;
    public float lineWidth = 10;

    public bool testRotate;

    Vector2[] dotOffset = new Vector2[4];


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Start()
    {
        pieceHolder = transform.parent;
        testPivotPoint.transform.localPosition = pivotPoint;

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

            rect.anchoredPosition = new Vector2(gridPosArray[i].x * dotSpacing - offset.x * dotSpacing, gridPosArray[i].y * dotSpacing - offset.y * dotSpacing);

            dotsArray[i] = targetDot;
            targetDot.Setup(targetPiece.dotTypes[i], this);
        }

        //Goes through each dot and measures grid distance to each other dot.
        // Distance is used to differentiate adjacent and diagonal dot connections. 
        for (int i = 0; i < gridPosArray.Length; i++)
        {
            //dotsArray[i].gameObject.transform.localPosition = gridPosArray[i];

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

        ////Dot offset
        //Vector2 dotMaxPosition = Vector2.zero;
        //Vector2 dotMinPosition = new Vector2(10, 10);
        //for (int i = 0; i < targetPiece.dotPositions.Length; i++)
        //{
        //    //Min
        //    if(dotMinPosition.x > targetPiece.dotPositions[i].x)
        //        dotMaxPosition.x = targetPiece.dotPositions[i].x;
        //    if(dotMaxPosition.y > targetPiece.dotPositions[i].y)
        //        dotMinPosition.y = targetPiece.dotPositions[i].y;
        //    //Max
        //    if (dotMaxPosition.x < targetPiece.dotPositions[i].x)
        //        dotMaxPosition.x = targetPiece.dotPositions[i].x;
        //    if (dotMaxPosition.y < targetPiece.dotPositions[i].y)
        //        dotMaxPosition.y = targetPiece.dotPositions[i].y;
        //}
        //dotOffset[0] = new Vector2(-dotMaxPosition.x * dotSpacing, dotMaxPosition.y * dotSpacing);
        //dotOffset[1] = new Vector2(dotMaxPosition.x * dotSpacing, dotMaxPosition.y * dotSpacing);
        //dotOffset[2] = new Vector2(dotMaxPosition.x * dotSpacing, -dotMaxPosition.y * dotSpacing);
        //dotOffset[3] = new Vector2(-dotMaxPosition.x * dotSpacing, -dotMaxPosition.y * dotSpacing);
    }
    public void EnforceDotPositions()
    {
        for (int i = 0;i < dotsArray.Length; i++)
        {
            RectTransform rect = dotsArray[i].GetComponent<RectTransform>();
            //Set position
            rect.localPosition = new Vector2(gridPosArray[i].x * dotSpacing, gridPosArray[i].y * dotSpacing);
            //Set rotation
            GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, rotationInt * -90);
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
        GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, rotationInt * -90);
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
}

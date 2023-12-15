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
    int testTimer = 0;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Start()
    {
        pieceHolder = transform.parent;
        testPivotPoint.transform.localPosition = pivotPoint;

    }
    public void Update()
    {
        testTimer++;
        if (testTimer >= 1000 && testRotate)
        {
            Debug.Log("In Update");
            Rotate();
            testTimer = 0;
        }
    }
    public void LoadPiece(LevelPiece targetPiece)
    {
        lineHolder = new GameObject();
        lineHolder.transform.SetParent(transform, false);

        //Set cordinats
        gridPosArray = targetPiece.dotPositions;

        //Setup dots
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
        for (int i = 0; i < dotsArray.Length; i++)
        {

            //dotsArray[i].gameObject.transform.localPosition = gridPosArray[i];

            if (!dotsArray[i].IsConnected)
            {
                List<int> diagonalList = new List<int>();
                bool foundAdjacent = false;
                for (int j = 0; j < dotsArray.Length; j++)
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
    }
    public void EnforceDotPositions()
    {
        for (int i = 0;i < dotsArray.Length; i++)
        {
            RectTransform rect = dotsArray[i].GetComponent<RectTransform>();
            //Set position
            rect.anchoredPosition = new Vector2(gridPosArray[i].x * dotSpacing, gridPosArray[i].y * dotSpacing);
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
            gridPosArray[i] = new Vector2((pivotPoint.x - (gridPosArray[i].y - pivotPoint.y)),
                (pivotPoint.y + (gridPosArray[i].x - pivotPoint.y)));

            dotsArray[i].relativePosition = gridPosArray[i] * dotSpacing;
            dotsArray[i].gameObject.transform.localPosition = dotsArray[i].relativePosition;
        }
        foreach (var line in connections)
        {
            line.UpdateLine();
        }
        rotationInt = (rotationInt + 1) % 4;
    }

    public void CreateLine(Dot dot1, Dot dot2)
    {
        GameObject newObject = new GameObject();
        CanvasRenderer cR = newObject.AddComponent<CanvasRenderer>();

        RectTransform newRectTrans = newObject.AddComponent<RectTransform>();
        newRectTrans.SetParent(lineHolder.transform, false);
        newRectTrans.anchoredPosition = gameObject.transform.localPosition;
        UILine uiLine = newObject.AddComponent<UILine>();
        RectTransform dot1Rect = dot1.GetComponent<RectTransform>();
        RectTransform dot2Rect = dot2.GetComponent<RectTransform>();
        connections.Add(uiLine);
        uiLine.Initialzie(dot1Rect, dot2Rect, lineWidth);
        dot1.IsConnected = true;
        dot2.IsConnected = true;
    }

    public void Place(Vector2 coordinates)
    {

    }

    public void Lift()
    {
        //Consider making a mathematical circle around the center of the piece. Lifting within the circle simply uses the lift location.
        //Lifting outside the circle snaps into the circle's edge.
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log($"Draging: {gameObject.name}");
        rectTransform.position = InputSystem.instance.touchPosition;
    }

    public void ReturnToHolder()
    {
        gameObject.transform.SetParent(pieceHolder);
        rectTransform.anchoredPosition = Vector2.zero;
    }
}

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

    public GameObject testPivotPoint;

    [SerializeField] private float dotSpacing;

    private List<LineRenderer> connections;

    private bool rotatable = true;

    private Transform pieceHolder;

    public bool testRotate;

    int testTimer = 0;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Start()
    {
        pieceHolder = transform.parent;
        LoadPiece();
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
    public void LoadPiece()
    {
        //Test piece
        Vector2[] dotCoordinats = new Vector2[]
            {new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0)};

        gridPosArray = dotCoordinats;

        Dot[] testDots = new Dot[3];
        dotsArray = new Dot[testDots.Length];

        for (int i = 0; i < testDots.Length; i++)
        {
            GameObject spawn = Instantiate(dotPrefab, transform);

            Dot targetDot = spawn.GetComponent<Dot>();

            testDots[i] = targetDot;
            dotsArray[i] = targetDot;

            RectTransform rect = spawn.GetComponent<RectTransform>();

            rect.anchoredPosition = new Vector2(dotCoordinats[i].x * dotSpacing, dotCoordinats[i].y * dotSpacing);

            switch(i)
            {
                case 0:
                    testDots[i].dotType = DotType.Red;
                    break;
                case 1:
                    testDots[i].dotType = DotType.Blue;
                    break;
                case 2:
                    testDots[i].dotType = DotType.Yellow;
                    break;
            }

            targetDot.Setup(testDots[i].dotType);
        }



        //Goes through each dot and measures grid distance to each other dot.
        // Distance is used to differentiate adjacent and diagonal dot connections. 
        for (int i = 0; i < dotsArray.Length; i++)
        {
            //
            dotsArray[i].gameObject.transform.localPosition = gridPosArray[i];

            if (!dotsArray[i].IsConnected)
            {
                List<int> diagonalList = new List<int>();
                bool foundAdjacent = false;
                for (int j = 0; j < dotsArray.Length; j++)
                {
                    if (i == j) continue;

                    float val = Vector2.Distance(gridPosArray[i], gridPosArray[j]);
                    if (val > 1.3f) continue;

                    else if (val < 1.1f && !foundAdjacent)
                    {
                        diagonalList.Add(j);

                    }
                    else if (val == 1)
                    {
                        foundAdjacent = true;
                        CreateConnection(dotsArray[i].gameObject, gridPosArray[i], gridPosArray[j]);
                        dotsArray[i].IsConnected = true;
                        dotsArray[j].IsConnected = true;
                    }
                }
                if (!foundAdjacent)
                {
                    foreach (int j in diagonalList)
                    {
                        CreateConnection(dotsArray[i].gameObject, gridPosArray[i], gridPosArray[j]);
                        dotsArray[i].IsConnected = true;
                        dotsArray[j].IsConnected = true;
                    }
                }
            }

        }

    }
    public void CreateConnection(GameObject gameObject, Vector2 start, Vector2 end)
    {
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        connections.Add(lineRenderer);
    }
    public void UpdateConnections()
    {

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

            dotsArray[i].gameObject.transform.localPosition = gridPosArray[i] * 100;
        }
    }

    public void Place(Vector2 coordinates)
    {

    }

    public void Lift()
    {

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
